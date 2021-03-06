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
    public class InitializeActorPositionsModule: OSMLSModule
    {
        protected override void Initialize()
        {
            //List<OsmClosedWay> buildings = MapObjects.GetAll<OsmClosedWay>().Where(x => x.Tags.ContainsKey("building")).ToList();
            //List<OsmNode> shops = MapObjects.GetAll<OsmNode>().Where(x => x.Tags.ContainsKey("shop")).ToList();
            //List<OsmNode> amenities = MapObjects.GetAll<OsmNode>().Where(x => x.Tags.ContainsKey("amenity")).ToList();
            //List<OsmClosedWay> everything = new List<OsmClosedWay>();
            //everything.AddRange(buildings);
            //everything.AddRange(shops);
            //everything.AddRange(amenities);


            PlaceState placeState = new PlaceState()
            {
                Home = new Point(new Coordinate(4172236, 7511796))
            };

            JobState jobState = new JobState()
            {
                Job = new Point(new Coordinate(4172939, 7512088))
            };
            Console.WriteLine($"Creating an actor {1}...");

            Actor actor = new Actor(placeState.Home.X + 100, placeState.Home.Y + 100);

            Console.WriteLine($"Home at {placeState.Home.X}, {placeState.Home.Y}. Placing an actor at {actor.X}, {actor.Y}");

            Console.WriteLine($"Job at {jobState.Job.X}, {jobState.Job.Y}");

            placeState.AddPlace(new Coordinate(4172797, 7511340), "building", "value");
            Console.WriteLine($"Favorite place at {placeState.FavoritePlaces[0].Coordinate.X}, {placeState.FavoritePlaces[0].Coordinate.Y}");

            placeState.AddPlace(new Coordinate(4174100, 7511664), "shop", "value");
            Console.WriteLine($"Favorite place at {placeState.FavoritePlaces[1].Coordinate.X}, {placeState.FavoritePlaces[1].Coordinate.Y}");

            placeState.AddPlace(new Coordinate(4172672, 7510798), "amenity", "value");
            Console.WriteLine($"Favorite place at {placeState.FavoritePlaces[2].Coordinate.X}, {placeState.FavoritePlaces[2].Coordinate.Y}");

            actor.AddState(jobState);
            actor.AddState(placeState);

            MapObjects.Add(actor);
            //////////////////////////////////////////////////////
            /*
            placeState = new PlaceState()
            {
                Home = new Point(new Coordinate(4173211, 7511938))
            };

            jobState = new JobState()
            {
                Job = new Point(new Coordinate(4172939, 7512088))
            };
            Console.WriteLine($"Creating an actor {2}...");

            actor = new Actor(placeState.Home.X + 100, placeState.Home.Y + 100);

            Console.WriteLine($"Home at {placeState.Home.X}, {placeState.Home.Y}. Placing an actor at {actor.X}, {actor.Y}");

            Console.WriteLine($"Job at {jobState.Job.X}, {jobState.Job.Y}");

            placeState.AddPlace(new Coordinate(4172797, 7511340), "building", "value");
            Console.WriteLine($"Favorite place at {placeState.FavoritePlaces[0].Coordinate.X}, {placeState.FavoritePlaces[0].Coordinate.Y}");

            placeState.AddPlace(new Coordinate(4174100, 7511664), "shop", "value");
            Console.WriteLine($"Favorite place at {placeState.FavoritePlaces[1].Coordinate.X}, {placeState.FavoritePlaces[1].Coordinate.Y}");

            placeState.AddPlace(new Coordinate(4172672, 7510798), "amenity", "value");
            Console.WriteLine($"Favorite place at {placeState.FavoritePlaces[2].Coordinate.X}, {placeState.FavoritePlaces[2].Coordinate.Y}");

            actor.AddState(jobState);
            actor.AddState(placeState);

            MapObjects.Add(actor);
            //////////////////////////////////////////////////////
            
            placeState = new PlaceState()
            {
                Home = new Point(new Coordinate(4173800, 7511408))
            };

            jobState = new JobState()
            {
                Job = new Point(new Coordinate(4172939, 7512088))
            };
            Console.WriteLine($"Creating an actor {3}...");

            actor = new Actor(placeState.Home.X + 100, placeState.Home.Y + 100);

            Console.WriteLine($"Home at {placeState.Home.X}, {placeState.Home.Y}. Placing an actor at {actor.X}, {actor.Y}");

            Console.WriteLine($"Job at {jobState.Job.X}, {jobState.Job.Y}");

            placeState.AddPlace(new Coordinate(4172797, 7511340), "building", "value");
            Console.WriteLine($"Favorite place at {placeState.FavoritePlaces[0].Coordinate.X}, {placeState.FavoritePlaces[0].Coordinate.Y}");

            placeState.AddPlace(new Coordinate(4174100, 7511664), "shop", "value");
            Console.WriteLine($"Favorite place at {placeState.FavoritePlaces[1].Coordinate.X}, {placeState.FavoritePlaces[1].Coordinate.Y}");

            placeState.AddPlace(new Coordinate(4172672, 7510798), "amenity", "value");
            Console.WriteLine($"Favorite place at {placeState.FavoritePlaces[2].Coordinate.X}, {placeState.FavoritePlaces[2].Coordinate.Y}");

            actor.AddState(jobState);
            actor.AddState(placeState);

            MapObjects.Add(actor);*/
        }
        public override void Update(long elapsedMilliseconds)
        {

        }
    }

    public class InitializeActorSpecsModule : OSMLSModule
    {
        protected override void Initialize()
        {
            var actors = MapObjects.GetAll<Actor>();
            Console.WriteLine($"Got {actors.Count} actors\n");

            foreach (var actor in actors)
            {
                SpecState specState = new SpecState()
                {
                    Health = 100,
                    Satiety = 100,
                    Mood = 100,
                    Stamina = 100,
                    Money = 10000,
                    Speed = 2
                };
                JobState jobState = actor.GetState<JobState>();

                jobState.AddJobTime(new TimeInterval(15, 00, 15, 30));
                jobState.AddJobTime(new TimeInterval(16, 00, 20, 30));

                actor.AddState(specState);

                Console.WriteLine($"Satiety is {actor.GetState<SpecState>().Satiety}, first job interval is {jobState.JobTimes[0].Start}");
            }
        }
        public override void Update(long elapsedMilliseconds)
        {

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
        public double Satiety;
        public double Mood;
        public double Stamina;
        public double Money;
        public double Speed;

        public SpecState() { }
        public SpecState(SpecState state)
        {
            // Исключение, если копируемое состояние - null
            if (state == null)
                throw new ArgumentNullException("state");

            Health = state.Health;
            Satiety = state.Satiety;
            Mood = state.Mood;
            Stamina = state.Stamina;
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
