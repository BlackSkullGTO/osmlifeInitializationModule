using System;
using System.Linq;
using System.Collections.Generic;

using NodaTime;

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
        private double radius = 1000;
        private double offset { get { return random.NextDouble() * 2 * radius - radius; } }

        protected override void Initialize()
        {
            List<OsmNode> buildings = MapObjects.GetAll<OsmNode>().Where(x => x.Tags.ContainsKey("building")).ToList();
            List<OsmNode> shops = MapObjects.GetAll<OsmNode>().Where(x => x.Tags.ContainsKey("shop")).ToList();
            List<OsmNode> amenities = MapObjects.GetAll<OsmNode>().Where(x => x.Tags.ContainsKey("amenity")).ToList();
            List<OsmNode> everything = new List<OsmNode>();
            everything.AddRange(buildings);
            everything.AddRange(shops);
            everything.AddRange(amenities);

            for (int i = 0; i < 3; i++)
            {
                SpecState specState = new SpecState()
                {
                    Health = random.Next(50, 100),
                    Hunger = random.Next(50, 100),
                    Mood = random.Next(50, 100),
                    Fatigue = random.Next(50, 100),
                    Money = random.Next(1000, 10000),
                    Speed = random.NextDouble() * 2 + 2
                };
                PlaceState placeState = new PlaceState()
                {
                    Home = new Point(buildings[random.Next(0, buildings.Count() - 1)].Coordinate)
                };
                JobState jobState = new JobState()
                {
                    Job = new Point(everything[random.Next(0, everything.Count() - 1)].Coordinate)
                };
                Console.WriteLine($"Creating an actor {i + 1}...");

                Actor actor = new Actor(placeState.Home.X + offset, placeState.Home.Y + offset);
                Console.WriteLine($"Home at {placeState.Home.X}, {placeState.Home.Y}. Placing an actor at {actor.X}, {actor.Y}");

                jobState.AddJobTime(new TimeInterval(10, 30, 13, 00));
                jobState.AddJobTime(new TimeInterval(15, 30, 18, 00));

                Console.WriteLine($"Job at {jobState.Job.X}, {jobState.Job.Y}");

                placeState.AddPlace(buildings[random.Next(0, buildings.Count() - 1)].Coordinate, "building", "value");
                Console.WriteLine($"First favorite place at {placeState.FavoritePlaces[0].Coordinate.X}, {placeState.FavoritePlaces[0].Coordinate.Y}");

                placeState.AddPlace(shops[random.Next(0, shops.Count() - 1)].Coordinate, "shop", "value");
                Console.WriteLine($"Second favorite place at {placeState.FavoritePlaces[1].Coordinate.X}, {placeState.FavoritePlaces[1].Coordinate.Y}");

                placeState.AddPlace(amenities[random.Next(0, amenities.Count() - 1)].Coordinate, "amenity", "value");
                Console.WriteLine($"Third favorite place at {placeState.FavoritePlaces[2].Coordinate.X}, {placeState.FavoritePlaces[2].Coordinate.Y}");

                // Добавляем компонент состояния. Внутри компонент копируется (тем самым методом copy), так что в принципе
                // можно всех акторов одним и тем же состоянием инициализировать
                actor.AddState(jobState);
                actor.AddState(placeState);
                actor.AddState(specState);

                // Добавляем актора в объекты карты
                MapObjects.Add(actor);
            }
        }
        public override void Update(long elapsedMilliseconds)
        {
            //Console.WriteLine("\nActorInitializeModule: Update");

            // Снова получаем список акторов
            //var actors = MapObjects.GetAll<Actor>();
            //Console.WriteLine($"Got {actors.Count} actors\n");

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

        public JobState()
        {
            JobTimes = new List<TimeInterval>();
        }
        public JobState(JobState state)
        {
            // Исключение, если копируемое состояние - null
            if (state == null)
                throw new ArgumentNullException("state");

            Job = state.Job;
            JobTimes = new List<TimeInterval>(state.JobTimes);
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
        public double Health;
        public double Hunger;
        public double Mood;
        public double Fatigue;
        public double Money;
        public double Speed;

        public SpecState() { }
        public SpecState(SpecState state)
        {
            // Исключение, если копируемое состояние - null
            if (state == null)
                throw new ArgumentNullException("state");

            Health = state.Health;
            Hunger = state.Hunger;
            Mood = state.Mood;
            Fatigue = state.Fatigue;
            Money = state.Money;
            Speed = state.Speed;
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

        public PlaceState()
        {
            FavoritePlaces = new List<Place>();
        }
        public PlaceState(PlaceState state)
        {
            // Исключение, если копируемое состояние - null
            if (state == null)
                throw new ArgumentNullException("state");

            Home = state.Home;
            FavoritePlaces = new List<Place>(state.FavoritePlaces);
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
