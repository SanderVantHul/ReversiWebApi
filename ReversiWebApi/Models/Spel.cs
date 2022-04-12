using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ReversiWebApi.Models
{
    public partial class Spel : ISpel
    {
        [NotMapped]
        private const int _BORD_OMVANG = 8;

        [NotMapped]
        private readonly int[,] _richting = new int[8, 2] {
                                {  0,  1 },         // naar rechts
                                {  0, -1 },         // naar links
                                {  1,  0 },         // naar onder
                                { -1,  0 },         // naar boven
                                {  1,  1 },         // naar rechtsonder
                                {  1, -1 },         // naar linksonder
                                { -1,  1 },         // naar rechtsboven
                                { -1, -1 } };       // naar linksboven

        private Kleur[,] _bord;

        [NotMapped]
        [JsonIgnore]
        public Kleur[,] Bord
        {
            get
            {
                return _bord;
            }
            set
            {
                _bord = value;
            }
        }

        public int ID { get; set; }

        [MaxLength(255)]
        public string Omschrijving { get; set; }

        [MaxLength(255)]
        public string Token { get; set; }

        [MaxLength(255)]
        public string Speler1Token { get; set; }

        [MaxLength(255)]
        public string Speler2Token { get; set; }

        public Kleur AandeBeurt { get; set; }

        public int AantalWit { get; set; }
        public int AantalZwart { get; set; }

        public Spel()
        {
            Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            Token = Token.Replace("/", "q");    // slash mijden ivm het opvragen van een spel via een api obv het token
            Token = Token.Replace("+", "r");    // plus mijden ivm het opvragen van een spel via een api obv het token

            Bord = new Kleur[_BORD_OMVANG, _BORD_OMVANG];
            Bord[3, 3] = Kleur.Wit;
            Bord[4, 4] = Kleur.Wit;
            Bord[3, 4] = Kleur.Zwart;
            Bord[4, 3] = Kleur.Zwart;
            Bord[2, 3] = Kleur.MogelijkeZet;
            Bord[3, 2] = Kleur.MogelijkeZet;
            Bord[4, 5] = Kleur.MogelijkeZet;
            Bord[5, 4] = Kleur.MogelijkeZet;

            AantalWit = 2;
            AantalZwart = 2;

            AandeBeurt = Kleur.Geen;
        }

        public void Pas()
        {
            // controleeer of er geen zet mogelijk is voor de speler die wil passen, alvorens van beurt te wisselen.
            if (IsErEenZetMogelijk(AandeBeurt))
                throw new Exception("Passen mag niet, er is nog een zet mogelijk");
            else
                WisselBeurt();
        }

        private void WisselBeurt() => AandeBeurt = AandeBeurt == Kleur.Wit ? Kleur.Zwart : Kleur.Wit;

        // return true als geen van de spelers een zet kan doen
        public bool Afgelopen()
        {
            RemoveHighlights();
            bool afgelopen = BordVol() || (!IsErEenZetMogelijk(Kleur.Wit) && !IsErEenZetMogelijk(Kleur.Zwart));
            HighlightMogelijkeZetten(AandeBeurt);
            return afgelopen;
        }

        public bool BordVol()
        {
            foreach (Kleur kleur in Bord)
            {
                if (kleur == Kleur.Geen) return false;
            }

            return true;
        }

        public Kleur OverwegendeKleur()
        {
            int aantalWit = 0;
            int aantalZwart = 0;
            for (int rijZet = 0; rijZet < _BORD_OMVANG; rijZet++)
            {
                for (int kolomZet = 0; kolomZet < _BORD_OMVANG; kolomZet++)
                {
                    if (_bord[rijZet, kolomZet] == Kleur.Wit)
                        aantalWit++;
                    else if (_bord[rijZet, kolomZet] == Kleur.Zwart)
                        aantalZwart++;
                }
            }
            if (aantalWit > aantalZwart)
                return Kleur.Wit;
            if (aantalZwart > aantalWit)
                return Kleur.Zwart;
            return Kleur.Geen;
        }

        public bool ZetMogelijk(int rijZet, int kolomZet)
        {
            if (!PositieBinnenBordGrenzen(rijZet, kolomZet))
                throw new Exception($"Zet ({rijZet},{kolomZet}) ligt buiten het bord!");
            return ZetMogelijk(rijZet, kolomZet, AandeBeurt);
        }

        public void DoeZet(int rijZet, int kolomZet)
        {
            RemoveHighlights();
            if (!ZetMogelijk(rijZet, kolomZet))
            {
                HighlightMogelijkeZetten(AandeBeurt);
                throw new Exception($"Zet ({rijZet},{kolomZet}) is niet mogelijk!");
            }

            for (int i = 0; i < _richting.GetLength(0); i++)
            {
                DraaiStenenVanTegenstanderInOpgegevenRichtingOmIndienIngesloten(rijZet, kolomZet, AandeBeurt, _richting[i, 0], _richting[i, 1]);
            }
            Bord[rijZet, kolomZet] = AandeBeurt;
            WisselBeurt();
            HighlightMogelijkeZetten(AandeBeurt);
            UpdateAantallen();
        }

        private void UpdateAantallen()
        {
            AantalZwart = 0;
            AantalWit = 0;
            foreach (var kleur in Bord)
            {
                switch (kleur)
                {
                    case Kleur.Wit:
                        AantalWit++;
                        break;
                    case Kleur.Zwart:
                        AantalZwart++;
                        break;
                    case Kleur.NewWit:
                        AantalWit++;
                        break;
                    case Kleur.NewZwart:
                        AantalZwart++;
                        break;
                }
            }
        }

        private static Kleur GetKleurTegenstander(Kleur kleur)
        {
            if (kleur == Kleur.Wit)
                return Kleur.Zwart;
            else if (kleur == Kleur.Zwart)
                return Kleur.Wit;
            else
                return Kleur.Geen;
        }

        private bool IsErEenZetMogelijk(Kleur kleur)
        {
            if (kleur == Kleur.Geen)
                throw new Exception("Kleur mag niet gelijk aan Geen zijn!");
            // controleeer of er een zet mogelijk is voor kleur
            for (int rijZet = 0; rijZet < _BORD_OMVANG; rijZet++)
            {
                for (int kolomZet = 0; kolomZet < _BORD_OMVANG; kolomZet++)
                {
                    if (ZetMogelijk(rijZet, kolomZet, kleur))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void HighlightMogelijkeZetten(Kleur kleur)
        {
            if (kleur == Kleur.Geen)
                throw new Exception("Kleur mag niet gelijk aan Geen zijn!");
            // controleer of er een zet mogelijk is voor kleur
            for (int rijZet = 0; rijZet < _BORD_OMVANG; rijZet++)
            {
                for (int kolomZet = 0; kolomZet < _BORD_OMVANG; kolomZet++)
                {
                    if (ZetMogelijk(rijZet, kolomZet, kleur))
                    {
                        Bord[rijZet, kolomZet] = Kleur.MogelijkeZet;
                    }
                }
            }
        }

        public void RemoveHighlights()
        {
            for (int rijZet = 0; rijZet < _BORD_OMVANG; rijZet++)
            {
                for (int kolomZet = 0; kolomZet < _BORD_OMVANG; kolomZet++)
                {
                    if (Bord[rijZet, kolomZet] == Kleur.MogelijkeZet)
                    {
                        Bord[rijZet, kolomZet] = Kleur.Geen;
                    }

                    if (Bord[rijZet, kolomZet] == Kleur.NewWit)
                    {
                        Bord[rijZet, kolomZet] = Kleur.Wit;
                    }

                    if (Bord[rijZet, kolomZet] == Kleur.NewZwart)
                    {
                        Bord[rijZet, kolomZet] = Kleur.Zwart;
                    }
                }
            }
        }

        private bool ZetMogelijk(int rijZet, int kolomZet, Kleur kleur)
        {
            // Check of er een _richting is waarin een zet mogelijk is. Als dat zo is, return dan true.
            for (int i = 0; i < 8; i++)
            {
                {
                    if (StenenInTeSluitenInOpgegevenRichting(rijZet, kolomZet,
                                                             kleur,
                                                             _richting[i, 0], _richting[i, 1]))
                        return true;
                }
            }
            return false;
        }

        private static bool PositieBinnenBordGrenzen(int rij, int kolom)
        {
            return (rij >= 0 && rij < _BORD_OMVANG &&
                    kolom >= 0 && kolom < _BORD_OMVANG);
        }

        private bool ZetOpBordEnNogVrij(int rijZet, int kolomZet)
        {
            // Als op het _bord gezet wordt, en veld nog vrij, dan return true, anders false
            return (PositieBinnenBordGrenzen(rijZet, kolomZet) && Bord[rijZet, kolomZet] == Kleur.Geen);
        }

        private bool StenenInTeSluitenInOpgegevenRichting(int rijZet, int kolomZet,
                                                          Kleur kleurZetter,
                                                          int rijRichting, int kolomRichting)
        {
            int rij, kolom;
            Kleur kleurTegenstander = GetKleurTegenstander(kleurZetter);
            if (!ZetOpBordEnNogVrij(rijZet, kolomZet))
                return false;

            // Zet rij en kolom op de index voor het eerst vakje naast de zet.
            rij = rijZet + rijRichting;
            kolom = kolomZet + kolomRichting;

            int aantalNaastGelegenStenenVanTegenstander = 0;
            // Zolang Bord[rij,kolom] niet buiten de bordgrenzen ligt, en je in het volgende vakje 
            // steeds de kleur van de tegenstander treft, ga je nog een vakje verder kijken.
            // Bord[rij, kolom] ligt uiteindelijk buiten de bordgrenzen, of heeft niet meer de
            // de kleur van de tegenstander.
            // N.b.: deel achter && wordt alleen uitgevoerd als conditie daarvoor true is.
            while (PositieBinnenBordGrenzen(rij, kolom) && Bord[rij, kolom] == kleurTegenstander)
            {
                rij += rijRichting;
                kolom += kolomRichting;
                aantalNaastGelegenStenenVanTegenstander++;
            }

            // Nu kijk je hoe je geeindigt bent met bovenstaande loop. Alleen
            // als alle drie onderstaande condities waar zijn, zijn er in de
            // opgegeven _richting stenen in te sluiten.
            return (PositieBinnenBordGrenzen(rij, kolom) &&
                    Bord[rij, kolom] == kleurZetter &&
                    aantalNaastGelegenStenenVanTegenstander > 0);
        }

        private bool DraaiStenenVanTegenstanderInOpgegevenRichtingOmIndienIngesloten(int rijZet, int kolomZet,
                                                                                     Kleur kleurZetter,
                                                                                     int rijRichting, int kolomRichting)
        {
            int rij, kolom;
            Kleur kleurTegenstander = GetKleurTegenstander(kleurZetter);
            bool stenenOmgedraaid = false;

            if (StenenInTeSluitenInOpgegevenRichting(rijZet, kolomZet, kleurZetter, rijRichting, kolomRichting))
            {
                rij = rijZet + rijRichting;
                kolom = kolomZet + kolomRichting;

                // N.b.: je weet zeker dat je niet buiten het _bord belandt,
                // omdat de stenen van de tegenstander ingesloten zijn door
                // een steen van degene die de zet doet.
                while (Bord[rij, kolom] == kleurTegenstander)
                {
                    Bord[rij, kolom] = kleurZetter == Kleur.Wit ? Kleur.NewWit : Kleur.NewZwart;
                    rij += rijRichting;
                    kolom += kolomRichting;
                }
                stenenOmgedraaid = true;
            }
            return stenenOmgedraaid;
        }
    }
}
