using System;
using System.Linq;
using System.Collections.Generic;

using CityDataExpansionModule;
using CityDataExpansionModule.OsmGeometries;

using OSMLSGlobalLibrary.Modules;
using OSMLSGlobalLibrary.Map;

using NetTopologySuite.Geometries;

using ActorModule;

namespace InitializeActorModule
{
    public class InitializeActorModule: OSMLSModule
    {
        private Random random = new Random();

        protected override void Initialize()
        {
            var buildings = MapObjects.GetAll<OsmNode>().Where(x => x.Tags.ContainsKey("building")).ToList();
            var shops = MapObjects.GetAll<OsmNode>().Where(x => x.Tags.ContainsKey("shop")).ToList();
            var amenities = MapObjects.GetAll<OsmNode>().Where(x => x.Tags.ContainsKey("amenity")).ToList();

            //int count = random.Next(5, 10);
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Creating an actor {i + 1}...");

                Point home = new Point(buildings[random.Next(0, buildings.Count()-1)].Coordinate);
                Actor actor = new Actor(home.X, home.Y);

                Console.WriteLine($"Home at {home.X}, {home.Y}. Placing an actor here");

                Point job = new Point(buildings[random.Next(0, buildings.Count() - 1)].Coordinate);

                Console.WriteLine($"Job at {job.X}, {job.Y}");

                SpecState specState = new SpecState();
                PlaceState placeState = new PlaceState(home);
                JobState jobState = new JobState(job);

                var place = buildings[random.Next(0, buildings.Count() - 1)];
                placeState.AddPlace(place.Coordinate, "building", "value");
                Console.WriteLine($"First favorite place at {place.Coordinate.X}, {place.Coordinate.Y}");

                place = shops[random.Next(0, shops.Count() - 1)];
                placeState.AddPlace(place.Coordinate, "shop", "value");
                Console.WriteLine($"Second favorite place at {place.Coordinate.X}, {place.Coordinate.Y}");

                place = amenities[random.Next(0, amenities.Count() - 1)];
                placeState.AddPlace(place.Coordinate, "amenity", "value");
                Console.WriteLine($"Third favorite place at {place.Coordinate.X}, {place.Coordinate.Y}");

                // Добавляем компонент состояния. Внутри компонент копируется (тем самым методом copy), так что в принципе
                // можно всех акторов одним и тем же состоянием инициализировать
                actor.AddState(specState);
                actor.AddState(placeState);
                actor.AddState(jobState);

                // Добавляем актора в объекты карты
                MapObjects.Add(actor);
            }
        }
        public override void Update(long elapsedMilliseconds)
        {
            Console.WriteLine("\nActorInitializeModule: Update");

            // Снова получаем список акторов
            var actors = MapObjects.GetAll<Actor>();
            Console.WriteLine($"Got {actors.Count} actors\n");

            // Для каждого актёра проверяем условия и назначаем новую активность если нужно
            /*foreach (var actor in actors)
            {
                // Достаём нужный компонент состояния
                SpecState state = actor.GetState<SpecState>();

                state.Hunger--;

                Console.WriteLine($"Actor has {state.Hunger} hunger points");
            }*/
        }
    }

    //Хранит интервалы времени работы, местоположение работы
    public class JobState: IState
	{
        public readonly List<TimeInterval> JobTimes;
        public Point Job;
        private Random random = new Random();
        public JobState(Point job)
        {
            Job = job;
            AddJobTime(new TimeInterval(10, 30, 13, 00));
            AddJobTime(new TimeInterval(15, 30, 18, 00));
        }
        public JobState(JobState state)
        {
            // Исключение, если копируемое состояние - null
            if (state == null)
                throw new ArgumentNullException("state");

        }

        // Выполняет копирование компонента, необходим для соответствия интерфейсу
        public IState Copy()
        {
            return new JobState(this);
        }
        public void AddJobTime(TimeInterval interval)
        {
            JobTimes.Add(interval);
        }
    }
    public class SpecState : IState
    {
        public int Health;
        public int Hunger;
        public int Mood;
        public int Fatigue;
        public int Money;
        public double Speed;
        private Random random = new Random();

        public SpecState()
        {
            Health = random.Next(50, 100);
            Hunger = random.Next(50, 100);
            Mood = random.Next(50, 100);
            Fatigue = random.Next(50, 100);
            Money = random.Next(1000, 10000);
            Speed = random.NextDouble() * 2 + 2;
        }
        public SpecState(SpecState state)
        {
            // Исключение, если копируемое состояние - null
            if (state == null)
                throw new ArgumentNullException("state");

            this.Health = state.Health;
            this.Hunger = state.Hunger;
            this.Mood = state.Mood;
            this.Fatigue = state.Fatigue;
            this.Money = state.Money;
            this.Speed = state.Speed;
        }

        // Выполняет копирование компонента, необходим для соответствия интерфейсу
        public IState Copy()
        {
            return new SpecState(this);
        }
    }
    public class Place: Point
	{
        public string TagKey;
        public string TagValue;
        public Place(double x, double y) : base(x, y) { }
        public Place(Coordinate coordinate) : base(new Coordinate(coordinate)) { }
    }
    public class PlaceState : IState
    {
        public Point Home;
        public readonly List<Place> FavoritePlaces;

        public PlaceState(Point home)
        {
            Home = home;
        }
        public PlaceState(PlaceState state)
        {
            // Исключение, если копируемое состояние - null
            if (state == null)
                throw new ArgumentNullException("state");

        }

        // Выполняет копирование компонента, необходим для соответствия интерфейсу
        public IState Copy()
        {
            return new PlaceState(this);
        }

        public void AddPlace(Place place)
		{
            FavoritePlaces.Add(place);
		}
        public void AddPlace(Coordinate coordinate, string tagKey, string tagValue)
        {
            Place place = new Place(coordinate);
            place.TagKey = tagKey;
            place.TagKey = tagValue;
            FavoritePlaces.Add(place);
        }
    }
}
