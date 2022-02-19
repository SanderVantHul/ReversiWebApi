using NUnit.Framework;
using ReversieISpelImplementatie.Model;
using ReversiWebApi.Models;
using System;
using System.Collections.Generic;

namespace ReversiTests
{
    [TestFixture]
    public class SpelRepositoryTest
    {
        SpelRepository _repo = new SpelRepository();

        [Test]
        public void GetSpellen_3spellen_return3Spellen()
        {
            //arrange
            _repo.Spellen.Clear();
            Spel spel1 = new Spel();
            Spel spel2 = new Spel();
            Spel spel3 = new Spel();
            _repo.Spellen.Add(spel1);
            _repo.Spellen.Add(spel2);
            _repo.Spellen.Add(spel3);

            //act
            List<Spel> spellen = _repo.GetSpellen();

            //assert
            Assert.IsNotEmpty(spellen);
            Assert.AreEqual(spellen.Count, 3);
            Assert.AreEqual(spellen[0], spel1);
            Assert.AreEqual(spellen[1], spel2);
            Assert.AreEqual(spellen[2], spel3);
        }

        [Test]
        public void AddSpel_voeg3SpellenToe_3SpellenToegevoegd()
        {
            //arrange
            _repo.Spellen.Clear();
            Spel spel1 = new Spel();
            Spel spel2 = new Spel();
            Spel spel3 = new Spel();

            //act
            _repo.AddSpel(spel1);
            _repo.AddSpel(spel2);
            _repo.AddSpel(spel3);

            //assert
            Assert.IsNotEmpty(_repo.Spellen);
            Assert.AreEqual(_repo.Spellen.Count, 3);
            Assert.AreEqual(_repo.Spellen[0], spel1);
            Assert.AreEqual(_repo.Spellen[1], spel2);
            Assert.AreEqual(_repo.Spellen[2], spel3);
        }
    }
}
