using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public static class EntityManager
    {
        public static List<Entity> entities;
        public static List<LivingEntity> livingEntities { get; set; }
        // TODO: Eventually add lists for monsters, animals, and NPCs

        static EntityManager()
        {
            entities = new List<Entity>();
            livingEntities = new List<LivingEntity>();
        }

        public static void add(Entity ent)
        {
            if (ent.GetType() == typeof(Entity)) entities.Add((Entity)Convert.ChangeType(ent, typeof(Entity)));
            if (ent.GetType() == typeof(LivingEntity)) livingEntities.Add((LivingEntity)Convert.ChangeType(ent, typeof(LivingEntity)));
        }

        public static Entity get(int i)
        {
            if (i > entities.Count - 1) return null;
            return entities[i];
        }
        public static T get<T>(int i) where T : Entity
        {
            if (typeof(T) == typeof(Entity))
            {
                if (i > entities.Count - 1) return null;
                return entities[i] as T;
            }
            if (typeof(T) == typeof(LivingEntity))
            {
                if (i > livingEntities.Count - 1) return null;
                return livingEntities[i] as T;
            }

            return default(T);
        }

        public static List<Entity> getEntities()
        {
            return entities;
        }
        public static List<T> getEntities<T>() where T : Entity
        {
            if (typeof(T) == typeof(Entity)) return entities as List<T>;
            if (typeof(T) == typeof(LivingEntity)) return livingEntities as List<T>;

            return default(List<T>);
        }

        public static void clear()
        {
            entities.Clear();
            livingEntities.Clear();
        }
    }
}
