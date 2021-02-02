using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public static class EntityManager
    {
        public static Dictionary<int, Entity> entities;
        public static Dictionary<string, Entity> entitiesByName;

        public static Dictionary<int, LivingEntity> livingEntities;
        public static Dictionary<string, LivingEntity> livingEntitiesByName;

        // TODO: Eventually add lists for monsters, animals, and NPCs

        static EntityManager()
        {
            entities = new Dictionary<int, Entity>();
            entitiesByName = new Dictionary<string, Entity>();

            livingEntities = new Dictionary<int, LivingEntity>();
            livingEntitiesByName = new Dictionary<string, LivingEntity>();
        }
        
        /// <summary>
        /// Adds a loaded entity into its matching entity list. 
        /// </summary>
        /// <param name="ent">The entity object being stored.</param>
        public static void add(Entity ent)
        {
            if (ent.GetType() == typeof(Entity))
            {
                Entity entity = (Entity)Convert.ChangeType(ent, typeof(Entity));
                entities.Add(entities.Count, entity);
                entitiesByName.Add(entity.name, entity);
            }

            if (ent.GetType() == typeof(LivingEntity))
            {
                LivingEntity lEntity = (LivingEntity)Convert.ChangeType(ent, typeof(LivingEntity));
                livingEntities.Add(livingEntities.Count, lEntity);
                livingEntitiesByName.Add(lEntity.name, lEntity);
            }
        }

        /// <summary>
        /// Gets an entity from the appropriate list based on the Entity type.
        /// </summary>
        /// <typeparam name="T">Either Entity or a subclass of Entity.</typeparam>
        /// <param name="i">The numerical index we are retrieving the entity from.</param>
        /// <returns>An entity matching the defined type of T.</returns>
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

        /// <summary>
        /// Gets an entity from the appropriate list using a namespace based on the Entity type.
        /// </summary>
        /// <typeparam name="T">Either Entity or a subclass of Entity.</typeparam>
        /// <param name="name">The entity namespace we are looking for.</param>
        /// <returns>An entity matching the defined type of T.</returns>
        public static T getByName<T>(string name) where T : Entity
        {
            if (typeof(T) == typeof(Entity))
            {
                if (name == null || !entitiesByName.ContainsKey(name)) return null;
                return entitiesByName[name] as T;
            }

            if (typeof(T) == typeof(LivingEntity))
            {
                if (name == null || !livingEntitiesByName.ContainsKey(name)) return null;
                return livingEntitiesByName[name] as T;
            }

            return default(T);

        }

        public static Dictionary<int, Entity> getEntities()
        {
            return entities;
        }

        public static void clear()
        {
            entities.Clear();
            entitiesByName.Clear();

            livingEntities.Clear();
            livingEntitiesByName.Clear();
        }
    }
}
