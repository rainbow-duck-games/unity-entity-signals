# Unity Entity Signals

<p>
  <a href="https://github.com/rainbow-duck-games/unity-entity-signals/actions?query=workflow%3A%22Unity+Tests%22"><img alt="Unity Tests status" src="https://github.com/rainbow-duck-games/unity-entity-signals/workflows/Unity Tests/badge.svg"></a>
  <a href="https://twitter.com/RainbowDuckGms"><img alt="Follow Twitter" src="https://img.shields.io/badge/Twitter-Follow-blue"></a>
  <a href="https://discord.gg/2b9BhDhVBJ"><img alt="Join Discord" src="https://img.shields.io/badge/Discord-Join-blueviolet"></a>
</p>

This project is an evolution of the library widely used in our projects [Pixeye Signals](https://github.com/PixeyeHQ/Unity3d-Signals). However
usign the library we found that we want something much more flexible to handle our cases. So we took best parts of the lib
and created the new one.

## The Idea

Everything is our projects were binded to one or another entities and we widely used signals to communicate entities changes
between components. First we used signals with entity reference inside, but the approach caused a giant collection of handlers
which started to be inefficient. After we started to create a new signals instance inside every entity, but this turns out
to be a mess of possible places to subscribe / send signals & caused some problems with generic messages.

All these problems triggered the development of this library. We also tried to achieve following targets:
- Strict type as much as possible
- Easy to debug and isolate
- Entity-related handlers
- Extendability
- Dynamic entity matching (in plans)
- Strict mode (in plans)
- Verbose mode (in plans)

## Base usage

#### Entity related handler
```
var es = new EntitySignals();
var entity = new TestEntity();
var handler = (ESHandler<TestEntity, int>) ((entity, i) => { /* Something useful here */ });

es.On(entity).Add(handler);
es.On(entity).Send(1);
```

#### Type related handler
```
var es = new EntitySignals();
var entity = new TestEntity();
var handler = (ESHandler<TestEntity, int>) ((entity, i) => { /* Something useful here */ });

es.On<TestEntity>().Add(handler);
es.On(entity).Send(1);
```

For detailed documentations check [README.md](/Packages/com.rainbow-duck-games.unity-entity-signals/README.md)

## Contribution

We are welcome any contribution. Feel free to create issues, start a discussion or even fork and create a pull request.
If you are looking for some advice or hints also consider to join our [Discord Server](https://discord.gg/2b9BhDhVBJ)

## Links & References

- [Pixeye Signals](https://github.com/PixeyeHQ/Unity3d-Signals) - The original library we used, also used as a reference
  for performance testing
- [Tact.Net](https://github.com/tdupont750/tact.net) - Used as part of this lib to optimize delegates calls. You can find it in [Utility/Tact](Packages/com.rainbow-duck-games.unity-entity-signals/Runtime/Utility/Tact)