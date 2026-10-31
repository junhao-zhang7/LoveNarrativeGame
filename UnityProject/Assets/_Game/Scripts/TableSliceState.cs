using System;
using System.Globalization;
using System.Text;

namespace LoveNarrativeGame.Prototype
{
    // Scene-local state: content assets are never used as a save file.
    public sealed class TableSliceState
    {
        public enum Step { Question, ClearDesk, ReadyToShare, Responding, MathReturned, EnglishReady, EnglishResponding, Complete }
        public Step Current { get; private set; }
        public int Layout { get; private set; }
        public int Hint { get; private set; }
        public bool TryAnswer(string value)
        {
            int number;
            return Current == Step.Question && int.TryParse((value ?? "").Normalize(NormalizationForm.FormKC).Trim(),
                NumberStyles.Integer, CultureInfo.InvariantCulture, out number) && number == 520 && FinishQuestion();
        }
        public int NextHint() { if (Current == Step.Question) Hint = Math.Min(2, Hint + 1); return Hint; }
        public bool FinishQuestion() { if (Current != Step.Question) return false; Current = Step.ClearDesk; return true; }
        public bool Clear(int layout)
        {
            if ((Current != Step.ClearDesk && Current != Step.ReadyToShare) || layout < 1 || layout > 2) return false;
            Layout = layout; Current = Step.ReadyToShare; return true;
        }
        public bool Share()
        {
            if (Current != Step.ReadyToShare && Current != Step.EnglishReady) return false;
            Current = Current == Step.ReadyToShare ? Step.Responding : Step.EnglishResponding; return true;
        }
        public bool ReturnPage()
        {
            if (Current != Step.Responding && Current != Step.EnglishResponding) return false;
            Current = Current == Step.Responding ? Step.MathReturned : Step.Complete; return true;
        }
        public bool StartEnglish() { if (Current != Step.MathReturned) return false; Current = Step.EnglishReady; return true; }
    }
}
