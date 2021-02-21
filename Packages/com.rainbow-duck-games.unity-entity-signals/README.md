# Unity Entity Signals

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