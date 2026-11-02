using System;
using System.Linq;
using LoveNarrativeGame.Prototype;
using UnityEditor;
using UnityEngine;

public static class StoryRouteChecks
{
    [MenuItem("Tools/Story Prototype/Check First Act Route")]
    public static void Run()
    {
        CheckState();
        var story = AssetDatabase.LoadAssetAtPath<StorySequence>(RearRowStoryDraft.AssetPath);
        if (story == null || story.beats.Length != 20) throw new Exception("First-act route content not migrated; author edits preserved.");
        int gate = Array.FindIndex(story.beats,b => b.requiresRoute);
        if (gate != 9 || story.beats.Count(b => b.requiresRoute) != 1 || story.beats[8].text != "我发现一条小道。" || story.beats[10].text != "从后面绕——" ||
            story.beats.Take(gate+1).Any(b => b.shot != StoryShot.BackRow) || story.beats[11].text != "他的手指刚划到纸边，身后传来声音。")
            throw new Exception("Route gate must occur before the teacher and second half of the sentence.");
        if (story.beats.Select(b => b.id).Distinct().Count() != 20) throw new Exception("Route chapter IDs invalid.");
        Debug.Log("STORY_ROUTE_CHECK_PASS: route adjacency, undo, reset, shortest-path proof, retry, replay, gate/story order; no player saves changed");
    }
    public static void CheckState()
    {
        var s = new StoryRouteState();
        Require(s.Path.Count==1 && s.Length==0 && !s.Submit(),"initial state");
        Require(!s.Visit(-1) && !s.Visit(100) && !s.Visit(5) && !s.Visit(0),"invalid jumps");
        Require(s.Visit(1) && s.Visit(2) && s.Visit(3) && s.Length==370 && !s.Submit(),"long route retries");
        Require(!s.Solved && s.AtGoal && !s.Visit(5),"failed submission stays at goal");
        s.Undo(); Require(!s.AtGoal && s.Length==290,"undo endpoint");
        s.Reset(); Require(s.Length==0 && s.Path.Count==1,"reset");
        s.Visit(4); s.Visit(6); Require(s.Visit(4) && s.Length==40 && s.Path.Count==2,"backtrack to visited place");
        s.Reset(); s.Visit(4); s.Visit(6); s.Visit(5); s.Visit(3); Require(s.Length==260 && !s.Submit(),"different detour rejected");
        s.Reset(); Require(s.Visit(4) && s.Visit(5) && s.Visit(3) && s.Length==160 && s.Submit(),"short route accepted");
        Require(s.Solved && !s.Submit() && !s.Visit(2),"duplicate success blocked");
        s.Undo(); Require(s.Solved && s.Length==160,"solved state immutable until reset");
        s.Reset(); Require(!s.Solved && !s.AtGoal,"replay resets");
        int minimum=int.MaxValue; int optimalCount=0;
        Enumerate(0,1,0,ref minimum,ref optimalCount);
        Require(minimum==StoryRouteState.ShortestLength && minimum==160 && optimalCount==1,"exhaustive graph proof");
    }
    private static void Enumerate(int current,int visited,int length,ref int minimum,ref int optimalCount)
    {
        if (current==3) { if(length<minimum) {minimum=length;optimalCount=1;} else if(length==minimum)optimalCount++; return; }
        for(int next=0;next<StoryRouteState.NodeCount;next++)
        { int cost=StoryRouteState.Distance(current,next); if(cost>0 && (visited & (1<<next))==0) Enumerate(next,visited|(1<<next),length+cost,ref minimum,ref optimalCount); }
    }
    private static void Require(bool ok,string message) { if(!ok)throw new Exception("Route check failed: "+message); }
}
