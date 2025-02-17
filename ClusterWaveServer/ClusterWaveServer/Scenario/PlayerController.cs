﻿using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using ClusterWaveServer.Scenes;
using ClusterWaveServer.Network;

namespace ClusterWaveServer.Scenario
{
    abstract class PlayerController
    {
        /// <summary>
        /// The player's body in the physics simulation. This can be used to get and set the position and velocity.
        /// <para>For changing rotation, see <see cref="PlayerController.rotation"/></para>
        /// </summary>
        protected Body body;

        /// <summary>
        /// The rotation the player is facing.
        /// </summary>
        protected float rotation;


        protected float health;

        /// <summary>
        /// The Player this PlayerController is assigned to
        /// </summary>
        protected PlayerInfo player;

        /// <summary>
        /// Gets the Player this PlayerController is assigned to
        /// </summary>
        public PlayerInfo Player { get { return player; } }

        /// <summary>
        /// The <see cref="ClusterWave.Scenes.InGameScene"/> object controlling this shiet
        /// </summary>
        protected InGameScene scene;

        /// <summary>
        /// Returns the position of this player's physics body
        /// </summary>
        public Vector2 Position { get { return body.Position; } }

        public float Rotation { get { return rotation; } set { rotation = value; } }

        public bool dead = false;

        /// <summary>
        /// Creates a PlayerController, assigning it a given position, scene and it's assigned Player.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="scene"></param>
        /// <param name="player"></param>
        public PlayerController(Vector2 position, InGameScene scene, PlayerInfo player)
        {
            rotation = 0;
            this.scene = scene;
            this.player = player;
            body = new Body(scene.Scenario.PhysicsWorld, position, 0f, this);
            body.CreateFixture(new FarseerPhysics.Collision.Shapes.CircleShape(Constants.PlayerColliderSize, Constants.PlayerDensity), this);
            body.BodyType = BodyType.Dynamic;
            body.FixedRotation = true;
            body.Friction = Constants.PlayerFriction;
            body.Restitution = Constants.PlayerRestitution;
            body.CollidesWith = Constants.PlayersCollideWith;
            body.CollisionCategories = Constants.PlayerCategory;
        }

        /// <summary>
        /// Override this method and apply any logic for damaging a player and checking whether it's dead.
        /// </summary>
        /// <param name="amount">The amount of damage the bullet inflicts</param>
        public abstract void Damage(float amount);

        public void UpdatePos(Vector2 pos)
        {
            body.Position = pos;
        }
    }
}
