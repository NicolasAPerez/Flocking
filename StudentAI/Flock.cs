using System.Collections.Generic;
using AI.SteeringBehaviors.Core;

namespace AI.SteeringBehaviors.StudentAI
{
    public class Flock
    {
        public float AlignmentStrength { get; set; }
        public float CohesionStrength { get; set; }
        public float SeparationStrength { get; set; }
        public List<MovingObject> Boids { get; protected set; }
        public Vector3 AveragePosition { get; set; }
        protected Vector3 AverageForward { get; set; }
        public float FlockRadius { get; set; }

        #region TODO:Complete
        public Flock()
        {
            //Unsure what, if anything, is needed here but I included it since its in the manual

        }
        //Code is based on the pseudocode in the video and slides provided to me

        //Calculate the direction which most of the flock is facing
        private Vector3 AlignmentAcc(MovingObject m)
        {
            Vector3 nextAcc = AverageForward / m.MaxSpeed;
            nextAcc = (nextAcc.Length > 1) ? Vector3.Normalize(nextAcc) : nextAcc;
            nextAcc *= AlignmentStrength;
            return nextAcc;
        }

        //Calculate the direction which turns towards the average center of the flock
        private Vector3 CohesionAcc(MovingObject m)
        {
            Vector3 nextAcc = AveragePosition - m.Position;
            float dist = nextAcc.Length;
            nextAcc.Normalize();

            if (dist < FlockRadius)
            {
                nextAcc *= dist / FlockRadius;
            }
            nextAcc *= CohesionStrength;
            return nextAcc;
        }

        //Calculates a vector which attempts to avoid collisions with the flock
        private Vector3 SeparationAcc(MovingObject m)
        {
            Vector3 nextAcc = Vector3.Zero;
            
            foreach (MovingObject boid in Boids)
            {
                if (boid != m)
                {
                    Vector3 vectorDist = m.Position - boid.Position;
                    float scalarDist = vectorDist.Length;
                    float safeDist = m.SafeRadius + boid.SafeRadius;

                    if (scalarDist < safeDist)
                    {
                        vectorDist.Normalize();
                        vectorDist *= (safeDist - scalarDist) / safeDist;
                        nextAcc += vectorDist;
                    }

                }
            }

            if (nextAcc.Length > 1f)
            {
                nextAcc.Normalize();
            }
            nextAcc *= SeparationStrength;
            return nextAcc;

        }


        public virtual void Update(float deltaTime)
        {
            //Update the two averages and save them to the class
            Vector3 avgForward = Vector3.Zero;
            Vector3 avgPos = Vector3.Zero;

            foreach(MovingObject m in Boids)
            {
                avgForward += m.Velocity;
                avgPos += m.Position;
            }
            
            AverageForward = avgForward / Boids.Count;
            AveragePosition = avgPos / Boids.Count;

            foreach(MovingObject m in Boids){
                //Calculate each of the Acceleration types and add them to the boid
                Vector3 nextAcc = Vector3.Zero;
                nextAcc = AlignmentAcc(m);
                nextAcc += CohesionAcc(m);
                nextAcc += SeparationAcc(m);
                float accMulti = m.MaxSpeed;
                nextAcc *= accMulti * deltaTime;
                m.Velocity += nextAcc;

                //Limit the velocity if it exceeds the max speed
                if (nextAcc.Length > m.MaxSpeed)
                {
                    nextAcc = Vector3.Normalize(m.Velocity);
                    m.Velocity = nextAcc * m.MaxSpeed;
                }
                m.Update(deltaTime);
            }
        
        }
        #endregion
    }
}
