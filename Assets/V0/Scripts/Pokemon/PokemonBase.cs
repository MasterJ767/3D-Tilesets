using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pokemon {
[CreateAssetMenu(fileName = "PKMN_", menuName = "ScriptableObjects/Pokemon", order = 1)]
public class PokemonBase : ScriptableObject
{
    public string pokemonName;
    [TextArea] public string description;
    
    public PokemonType type1;
    public PokemonType type2;

    public int baseHP;
    public int baseAttack;
    public int baseDefence;
    public int baseSpecialAttack;
    public int baseSpecialDefence;
    public int baseSpeed;
}

[Serializable]
public enum PokemonType 
{
    None,
    Normal,
    Grass,
    Fire,
    Water,
    Electric,
    Flying,
    Bug,
    Fighting,
    Rock, 
    Ground,
    Poison,
    Psychic,
    Ghost,
    Ice,
    Dragon,
    Dark,
    Steel,
    Fairy
}
}