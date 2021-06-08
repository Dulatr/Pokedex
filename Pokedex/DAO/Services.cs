using System;
using System.Collections.Generic;

// Installed References
using SQLite;
using PokeApiNet;

// Local References
using Pokedex.Models;

namespace Pokedex.DAO
{
    /// <summary>
    /// This is the data access object. It does fun stuff with SQL and
    /// and the PokeAPI. It holds the database reference here, and 
    /// shouldn't interact with it any other way.
    /// </summary>
    public class Services
    {
        public Services(SQLiteConnection dbRef)
        {
            DB = dbRef;
        }

        /// <summary>
        /// Grab a pokemon by it's associated ID.
        /// No ID provided will return a random first gen. 
        /// </summary>
        /// 
        /// <param name="ID">The index of a pokemon in the pokedex: Max 151, Min 1.</param>
        /// 
        /// <returns>First generation pokemon from database.</returns>
        public Models.Pokemon getAPokemon(int ID = 0)
        {
            if (ID == 0)
            {
                Random random = new Random();
                ID = random.Next(1,151);
            }
            else if (ID > 151 || ID < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(ID),$"Expected value between 151 and 0. Received: {ID}");
            }

            var poke = DB.Query<Models.Pokemon>($"SELECT * FROM pokemon WHERE id={ID}");

            if (poke == null)
                return new Models.Pokemon();

            return poke.ToArray()[0];
        }

        /// <summary>
        /// Grab a pokemon by it's identifying name. If not found, will return pokemon instance
        /// with an ID of 0.
        /// </summary>
        /// 
        /// <param name="identifier">The name of the pokemon you wish to pull.</param>
        /// 
        /// <returns>First generation pokemon from database.</returns>
        public Models.Pokemon getAPokemon(string identifier)
        {
            if (identifier == null)
                identifier = 0.ToString();

            var poke = DB.Query<Models.Pokemon>("SELECT * FROM pokemon WHERE identifier='" + identifier.ToLower() + "'");
            
            if (poke == null || poke.Count == 0)
                return new Models.Pokemon() { ID = 0 };

            return poke.ToArray()[0];
        }

        /// <summary>
        /// Conduct a 'fuzzy' search for pokemon matching the specific search string.
        /// Looks for ID, Identifier, or Type.
        /// </summary>
        /// 
        /// <param name="searchstring">A general string search term.</param>
        /// 
        /// <returns>List of result objects describing first gen pokemon who match the query.</returns>
        public List<Models.Result> getPokemon(string searchstring)
        {

            if (searchstring == null || searchstring == "")
            {
                return new List<Models.Result>();
            }

            var pokeList = DB.Query<Models.Result>(
                @"SELECT pokemon.id, pokemon.identifier, types.identifier AS typename FROM pokemon 
	                INNER JOIN pokemon_types ON pokemon.id = pokemon_types.pokemon_id
	                INNER JOIN types ON pokemon_types.type_id = types.id
                  WHERE types.identifier LIKE '%" + searchstring.ToLower() + "%'" +
	                "OR pokemon.id LIKE '%" + searchstring.ToLower() + "%'" +
	                "OR pokemon.identifier LIKE '%" + searchstring.ToLower() + "%';"
            );

            return pokeList;
        }

        /// <summary>
        /// Return the type identifier of a pokemon with given ID. 
        /// No ID provided will return empty string.
        /// 
        /// </summary>
        /// <param name="ID">The index of a pokemon in the pokedex: Max 151, Min 1.</param>
        /// 
        /// <returns>A pokemons type identifier as a string.</returns>
        public string getPokemonTypeClass(int ID = 0)
        {
            if (ID == 0)
            {
                Random random = new Random();
                ID = random.Next(1, 151);
            }
            
            else if (ID < 1 || ID > 151)
            {
                throw new ArgumentOutOfRangeException(nameof(ID), $"Expected value between 151 and 0. Received: {ID}"); 
            }

            var typeClass = DB.Query<Models.Types>(
                $@"SELECT pokemon.id,pokemon.identifier,types.identifier FROM pokemon
	                INNER JOIN pokemon_types ON pokemon.id = pokemon_types.pokemon_id
	                INNER JOIN types ON pokemon_types.type_id=types.id
                   WHERE pokemon.id={ID};"
            );

            if (typeClass == null || typeClass.Count == 0)
                return "";
            else if (typeClass.Count > 1)
                return $" {typeClass[0].Identifier} / {typeClass[1].Identifier}";
            else
                return $" {typeClass[0].Identifier}";
        }

        /// <summary>
        /// Creates database tables if they don't exist.
        /// 
        /// Might be slow for first time loading application,
        /// but subsequent launches will be much faster with a
        /// filled database.
        /// </summary>
        public async void InitializeTables()
        { 
            DB.CreateTable<Models.Pokemon>();
            DB.CreateTable<Models.PokemonTypes>();
            DB.CreateTable<Models.Types>();
            
            // Get all pokemon from api
            if (DB.Query<Models.Pokemon>("SELECT * FROM pokemon").Count == 0)
            {
                var _pokemonResources = await ApiClient.GetNamedResourcePageAsync<PokeApiNet.Pokemon>(151,0);
                List<Models.Pokemon> _creatures = new List<Models.Pokemon>();

                foreach (var _url in _pokemonResources.Results)
                {
                    var _creature = await ApiClient.GetResourceAsync(_url);
                    _creatures.Add(new Models.Pokemon()
                    {
                        Identifier = _creature.Name,
                        Species_id = _creature.Id,
                        Height = _creature.Height,
                        Weight = _creature.Weight,
                        Base_Experience = _creature.BaseExperience,
                        Order = _creature.Order,
                        Is_Default = _creature.IsDefault,
                        Sprite = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{_creature.Id}.png"
                    });                    
                }
                DB.InsertAll(_creatures);
            }

            // Obtain possible types 
            if (DB.Query<Models.Types>("SELECT * FROM types").Count == 0)
            {
                var _typeResources = await ApiClient.GetNamedResourcePageAsync<PokeApiNet.Type>();
                List<Models.Types> _types = new List<Types>();

                foreach (var _url in _typeResources.Results)
                {
                    var _type = await ApiClient.GetResourceAsync(_url);
                    var _generation = await ApiClient.GetResourceAsync(_type.Generation);

                    var _dmg_Class_Resource = _type.MoveDamageClass;

                    // Some resources might be null, so this handles
                    // whether to attempt to gather the resource. Null 
                    // values are filled to <-1>
                    MoveDamageClass _dmg_Class;
                    if (_dmg_Class_Resource == null)
                        _dmg_Class = null;
                    else
                        _dmg_Class = await ApiClient.GetResourceAsync(_type.MoveDamageClass);              

                    _types.Add(new Models.Types()
                    {
                        ID = _type.Id,
                        Identifier = _type.Name,
                        Generation_ID = _generation.Id,
                        Damage_Class_ID = _dmg_Class == null ? -1 : _dmg_Class.Id
                    });
                }

                DB.InsertAll(_types);
            }

            // Obtain a list of pokemon id's with their associated types
            // This part is a bit messy due to the Api not allowing 
            // to grab the PokemonType resources directly.
            if (DB.Query<Models.PokemonTypes>("SELECT * FROM pokemon_types").Count == 0)
            {
                List<Models.PokemonTypes> _pokemonTypeList = new List<PokemonTypes>();

                foreach (Models.Pokemon pokemon in DB.Query<Models.Pokemon>("SELECT * FROM pokemon"))
                {
                    var _pokemonResources = await ApiClient.GetResourceAsync<PokeApiNet.Pokemon>(pokemon.ID);
                    var _pokemonTypes = _pokemonResources.Types;
                    foreach (var _type in _pokemonTypes)
                    {
                        var _typeInfo = await ApiClient.GetResourceAsync<PokeApiNet.Type>(_type.Type);
                        _pokemonTypeList.Add(new Models.PokemonTypes()
                        {
                            Pokemon_ID = pokemon.ID,
                            Type_ID = _typeInfo.Id,
                            Slot = _type.Slot
                        });
                    }
                }

                DB.InsertAll(_pokemonTypeList);
            }
        }

        /// <summary>
        /// Primary Api client used by the DAO.
        /// </summary>
        private PokeApiClient _apiClient;
        private PokeApiClient ApiClient
        {
            get
            {
                if (_apiClient == null)
                    _apiClient = new PokeApiClient();
                return _apiClient;
            }
        }

        /// <summary>
        /// Holds primary instance of data base. Only the DAO should have access to this property.
        /// </summary>
        private SQLiteConnection _db;
        private SQLiteConnection DB
        {
            get
            {
                return _db;
            }
            set
            {
                _db = value;
            }
        }
        
    }
}
