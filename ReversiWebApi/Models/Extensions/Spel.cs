using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace ReversiWebApi.Models
{
    public partial class Spel
    {
        [JsonPropertyName("bord")]
        [Column("Bord")]
        [MaxLength (255)]
        public string StringBord  
        {
            get => JsonConvert.SerializeObject(Bord); // string representatie van Bord property

            set => Bord = JsonConvert.DeserializeObject<Kleur[,]>(value); // zet string representatie om naar Kleur[,]
        }
    }
}