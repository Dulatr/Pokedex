using System.Collections.Generic;
using System.Windows.Input;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using Pokedex.Models;
using Windows.UI.Xaml.Controls;

namespace Pokedex.ViewModels
{
    public class MainPageVM : ObservableObject
    {

        public MainPageVM()
        {
            PressedA = new RelayCommand(NextPokemon);
            PressedB = new RelayCommand(PreviousPokemon);

            // Register the VM as a recipient
            WeakReferenceMessenger.Default.Register<ValueChangedMessage<bool>>(this, 
                (r, msg) => 
                {
                    IsBusy = msg.Value;

                    if (IsBusy == false && _firstLoad)
                    {
                        Creature = App.Servicer.getAPokemon(_index);
                        PokemonType = App.Servicer.getPokemonTypeClass(_index);
                        SpriteURL = Creature.Sprite;
                        _firstLoad = !_firstLoad;
                    }
                }
            );

            WeakReferenceMessenger.Default.Register<ValueChangedMessage<string>>(this,
                (r,msg) => { LoadStatus = msg.Value; }
            );
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
            SpriteURL = Creature.Sprite;
        }

        public void PreviousPokemon()
        {
            if (_index == 1)
                _index = 151;
            else 
                _index--;

            Creature = App.Servicer.getAPokemon(_index);
            PokemonType = App.Servicer.getPokemonTypeClass(_index);
            SpriteURL = Creature.Sprite;
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
            SpriteURL = Creature.Sprite;

            _index = results[0].ID;

            Suggestions = results;
        }

        private int _index = 1;
        private bool _firstLoad = true;

        private string _loadStatus = "Starting up the program";
        public string LoadStatus
        {
            get { return _loadStatus; }
            set { SetProperty(ref _loadStatus, value, nameof(LoadStatus)); }
        }
        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value, nameof(IsBusy)); }
        }

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
        private string _spriteURL;
        public string SpriteURL
        {
            get
            {
                return _spriteURL;
            }
            set
            {
                SetProperty(ref _spriteURL, value, nameof(SpriteURL));
            }
        }
    }
}
