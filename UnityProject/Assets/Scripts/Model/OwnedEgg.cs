using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/** Wrapper for an egg, with an additional specifier
 * for which friend, if any, is holding on to the egg. */
public class OwnedEgg
{
    /** The friend holding on to the egg. 
     * Must be null if the egg is at its owner. */
    public string _friend;

    public Egg _egg;
}
