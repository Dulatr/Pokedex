﻿using System.Collections.Generic;
using System.Windows.Input;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Pokedex.Models;
using Windows.UI.Xaml.Controls;

namespace Pokedex.ViewModels
{
    public class MainPageVM : ObservableObject
    {

        public MainPageVM()
        {
            Creature = App.Servicer.getAPokemon(_index);
            PokemonType = App.Servicer.getPokemonTypeClass(_index);

            PressedA = new RelayCommand(NextPokemon);
            PressedB = new RelayCommand(PreviousPokemon);
        }

        public RelayCommand PressedA { get; set; }
        public RelayCommand PressedB { get; set; }

        public void NextPokemon()
        {
            if (_index == 151)
                _index = 1;
            else
                _index++;

            Creature = App.Servicer.getAPokemon(_index);
            PokemonType = App.Servicer.getPokemonTypeClass(_index);
        }

        public void PreviousPokemon()
        {
            if (_index == 1)
                _index = 151;
            else 
                _index--;

            Creature = App.Servicer.getAPokemon(_index);
            PokemonType = App.Servicer.getPokemonTypeClass(_index);
        }

        /// <summary>
        /// Fuzzy search for pokemon. Looks for ID # or identifier.
        /// Updates Creature context if found.
        /// </summary>
        public void SearchPokemon()
        {
            var results = App.Servicer.getPokemon(SearchString);

            if (results.Count == 0)
            {
                // pokemon not found
                Suggestions = new List<Result>()
                {
                    new Result() { Identifier = "No Results Found" }
                };
                return;
            }

            Creature = App.Servicer.getAPokemon(results[0].ID);
            PokemonType = results[0].TypeName;
            _index = results[0].ID;

            Suggestions = results;
        }

        private int _index = 1;

        private string _searchString;
        public string SearchString
        {
            get
            {
                return _searchString;
            }
            set
            {
                SetProperty(ref _searchString, value, nameof(SearchString));
                SearchPokemon();
            }
        }

        private List<Models.Result> _suggestions;
        public List<Models.Result> Suggestions
        {
            get
            {
                if (_suggestions == null)
                    _suggestions = new List<Result>();
                return _suggestions;
            }
            set
            {
                SetProperty(ref _suggestions, value, nameof(Suggestions));
            }
        }

        private string _pokemonType;
        public string PokemonType
        {
            get
            {
                return _pokemonType;
            }
            set
            {
                SetProperty(ref _pokemonType, value, nameof(PokemonType));
            }
        }
        private Pokemon _pokemon;
        public Pokemon Creature
        {
            get
            {
                if (_pokemon == null)
                {
                    _pokemon = new Pokemon();
                }
                return _pokemon;
            }
            set
            {
                SetProperty(ref _pokemon, value, nameof(Creature));
            }
        }
    }
}
