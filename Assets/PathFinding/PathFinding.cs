using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFinding
{
    class PathFindingLogic
    {

        // Cache
        private static CollisionMap targetMap;
        private static List<PlayerMovementPath> cachedSolutions = new List<PlayerMovementPath>();


        private static PlayerMovementPath getCachedSolution(CollisionMap Map)
        {
            if (cachedSolutions.Count==0)
            {
                return null;
            }

            //Debug.Log(cachedSolutions.Count);

            return cachedSolutions.Find(path => path.StartCoordinate.Equals(Map.CurrentPosition) && path.GoalCoordinate.Equals(Map.GoalPosition));
        }


        public static PlayerMovementPath ProcessSolution(CollisionMap Map)
        {
            PlayerMovementPath currentPath = null;
            List<PlayerMovementPath> statesToCheck = new List<PlayerMovementPath>();
            PlayerMovementPath solution = null;

            // If we have this map saved, we can attempt to get the solution from cache
            if (Map.Equals(targetMap)){
                // Attempt to get it from cache
                solution = getCachedSolution(Map);
                if (solution != null)
                {
                    //return solution;
                }
            }else{
                targetMap = Map;
                cachedSolutions.Clear();
            }

            if (!isGoalReachable(Map))
            {
                return null;
            }

            int stateCount = 0;
            //Debug.Log("ProcessSolution");
            while (true)
            {
                
                if (currentPath == null)
                {
                    currentPath = new PlayerMovementPath();
                    currentPath.CoordinatePath.Add(Map.CurrentPosition);
                    currentPath.HeuristicValue = Map.getElement(Map.CurrentPosition).DistanceFromGoal;
                }
                else
                {
                    if (statesToCheck.Count() == 0)
                    {
                        // If we have searched through all possible states, we cannot reach goal. End
                        break;
                    }

                    int heuristicValue = statesToCheck.Min(path => path.HeuristicValue);

                    currentPath = statesToCheck.Where(path => path.HeuristicValue == heuristicValue).FirstOrDefault();

                    // Do not need to check any states that are currently looking at this position
                    statesToCheck.RemoveAll(path => path.LastCoordinate == currentPath.LastCoordinate);
                }
                // We have found the solution
                if (Map.GoalPosition.Equals(currentPath.getLastCoordinate()))
                {
                    solution = currentPath;
                    break;
                }
                stateCount++;
                // Add all states to the list
                statesToCheck.AddRange(createProposedMovementPaths(currentPath, Map));
            }

            //Debug.Log("ProcessSolution end");

            if (solution != null)
            {
                solution.CoordinatePath.RemoveAt(0);
                solution.StartCoordinate = Map.CurrentPosition;
                solution.GoalCoordinate = Map.GoalPosition;
                //Debug.Log("adding solution");
                cachedSolutions.Add(solution);
            }

            return solution;
        }

        private static bool isGoalReachable(CollisionMap Map)
        {

            List<CollisionMapElement> neighboursToCheck = Map.getElement(Map.GoalPosition).getUnblockedNeighbours();
            List<CollisionMapElement> elementList = new List<CollisionMapElement>();
            //Debug.Log("isGoalReachable");
            while (neighboursToCheck.Except(elementList).Count() > 0)
            {
                
                CollisionMapElement target = neighboursToCheck[0];
                if (!elementList.Contains(target))
                    elementList.Add(target);

                List<CollisionMapElement> neighbours = target.getUnblockedNeighbours();
                
                if (
                     neighboursToCheck.Except(neighbours).Count() != 0 &&
                     elementList.Intersect(neighbours).Count() != neighbours.Count()
                    )
                {
                    neighboursToCheck.AddRange(neighbours.Except(neighboursToCheck).ToList());
                }
                neighboursToCheck.Remove(target);

            }
            //Map.printMap();
            //Debug.Log("end isGoalReachable");

            return elementList.Any(element => element.ElementCoordinate.Equals(Map.CurrentPosition));
        }

        private static List<PlayerMovementPath> createProposedMovementPaths(PlayerMovementPath currentMovementPath, CollisionMap Map) 
        {
            List<PlayerMovementPath> paths = new List<PlayerMovementPath>();

            // Get the unblocked neighbours at the current coordinate
            List<CollisionMapElement> childElements = Map.getElement(currentMovementPath.getLastCoordinate()).getUnblockedNeighbours();

            foreach (CollisionMapElement element in childElements)
            {
                // Do not go back the direction you just came
                if (!element.ElementCoordinate.Equals(currentMovementPath.getSecondToLastCoordinate()))
                {
                    PlayerMovementPath newMovementPath = currentMovementPath.Clone();
                    // Add the other coordinates
                    newMovementPath.CoordinatePath.Add(element.ElementCoordinate);
                    newMovementPath.HeuristicValue = Map.getElement(element.ElementCoordinate).DistanceFromGoal + currentMovementPath.getDistanceTravelled();
                    paths.Add(newMovementPath);
                }

            }

            return paths;
        }

    }


}
