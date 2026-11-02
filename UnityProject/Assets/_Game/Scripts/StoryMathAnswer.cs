using System.Globalization;
using System.Text;

namespace LoveNarrativeGame.Prototype
{
    public static class StoryMathAnswer
    {
        public static bool IsCorrect(string text)
        {
            int value;
            return int.TryParse((text ?? "").Normalize(NormalizationForm.FormKC).Trim(),
                NumberStyles.Integer, CultureInfo.InvariantCulture, out value) && value == 520;
        }
    }
}
