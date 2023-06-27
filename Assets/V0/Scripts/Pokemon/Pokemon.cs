using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pokemon {
public class Pokemon : MonoBehaviour
{
    private PokemonBase _base;
    private int level;

    public int Attack => Mathf.FloorToInt((Mathf.FloorToInt((2 * _base.baseAttack) * level / 100) + 5));
    public int Defence => Mathf.FloorToInt((Mathf.FloorToInt((2 * _base.baseDefence) * level / 100) + 5));
    public int SpecialAttack => Mathf.FloorToInt((Mathf.FloorToInt((2 * _base.baseSpecialAttack) * level / 100) + 5));
    public int SpecialDefense => Mathf.FloorToInt((Mathf.FloorToInt((2 * _base.baseSpecialDefence) * level / 100) + 5));
    public int Speed => Mathf.FloorToInt((Mathf.FloorToInt((2 * _base.baseSpeed) * level / 100) + 5));
    public int MaxHP => Mathf.FloorToInt((2 * _base.baseSpeed) * level / 100) + level + 10;

    public Pokemon(PokemonBase pBase, int pLevel) {
        _base = pBase;
        level = pLevel;
    }
}
}