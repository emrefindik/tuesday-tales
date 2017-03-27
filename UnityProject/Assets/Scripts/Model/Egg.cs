using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class Egg
{
    public string _name;
    public double _size;

    // Locations that this egg can hatch.
    // TODO if we can anyhow convert this to "any beach" as opposed to a list of specific locations, that would be great
    public List<Location> _hatchLocations;

    // TODO add other relevant fields
}
