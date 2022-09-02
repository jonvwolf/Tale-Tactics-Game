using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ServerModels
{
    public class HmCommandModel
    {
        public List<long> AudioIds { get; set; }
        public long? ImageId { get; set; }
        public long? MinigameId { get; set; }
        public long? Timer { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            var list = string.Join(", ", AudioIds ?? new List<long>());
            return $"ImageId: {ImageId} Audios: {list} MinigameId: {MinigameId} Timer: {Timer} Text: {Text}";
        }
    }
}
