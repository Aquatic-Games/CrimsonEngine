using BepuPhysics;
using BepuPhysics.Collidables;
using Crimson.Physics.Shapes.Descriptions;

namespace Crimson.Physics.Shapes;

public class BoxShape : Shape
{
    internal BoxShape(Simulation simulation, TypedIndex index) : base(simulation, index) { }
}