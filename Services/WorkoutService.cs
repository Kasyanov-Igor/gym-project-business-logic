﻿using System.Linq;
using Model.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using gym_project_business_logic.Model.Domains;
using gym_project_business_logic.Services.Interface;

namespace gym_project_business_logic.Services
{
	public class WorkoutService : IWorkoutService
	{
		private ADatabaseConnection _connection;
		public WorkoutService(ADatabaseConnection databaseConnection)
		{
			this._connection = databaseConnection;
		}

		public async Task<IEnumerable<Workout>> GetWorkoutsByCoach(int id)
		{
			var allWorkouts = await this._connection.Workouts.ToListAsync();

			return allWorkouts.Where(workout => workout.CoachId == id);
		}

		public async Task<IEnumerable<Workout>> GetWorkoutsByGym(int id)
		{
			var allWorkouts = await this._connection.Workouts.ToListAsync();

			return allWorkouts.Where(workout => workout.GymId == id);
		}

		public async Task<bool> UpdateClientNameAsync(int workoutId, DTOWorkout? newWorkout)
		{
			var workout = await _connection.Workouts.FindAsync(workoutId);
			if (workout == null)
			{
				return false; // не найден
			}

			if (newWorkout != null)
			{
				workout.Title = newWorkout.Title;
				workout.NameCoach = newWorkout.NameCoach;
				workout.GymId = newWorkout.GymId;
				workout.Places = newWorkout.Places;
				workout.ClientName = newWorkout.ClientName;
				workout.Description = newWorkout.Description;
				workout.DurationMinutes = newWorkout.DurationMinutes;
				workout.BookingTime = newWorkout.BookingTime;
				workout.CoachId = newWorkout.CoachId;

				try
				{
					await _connection.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!this._connection.Workouts.Any(w => w.Id == workoutId))
					{
						return false;
					}
					throw;
				}
			}

			return true;
		}

		public async Task<bool> AddClient(int workoutId, string? newClient)
		{
			var workout = await this._connection.Workouts.FindAsync(workoutId);
			if (workout == null)
			{
				return false; // не найден
			}

			if (newClient != null)
			{
				workout.ClientName = newClient;
				if (workout.Places > 0)
				{
					workout.Places -= 1;
				}
				else
				{
					return false;
				}

				try
				{
					await _connection.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!this._connection.Workouts.Any(w => w.Id == workoutId))
					{
						return false;
					}
					throw;
				}
			}

			return true;
		}

	}
}
