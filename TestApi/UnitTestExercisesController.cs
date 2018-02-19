using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptiflowApi.Controllers;
using OptiflowApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestApi
{
    [TestClass]
    public class UnitTestExercisesController
    {
        ExercisesController exercisesController = new ExercisesController("exercisetest");
        Exercise exercise = new Exercise(0, "5 min. lopen", "loop zo hard als je kunt gedurende 5 minuten", "imagetest");
        Exercise exercise2 = new Exercise(1, "10 min. stilzitten", "zit stil gedurende 10 minuten", "imagetest2");

        public async Task CleanTable()
        {
            int amount = await exercisesController.CountExercises();
            int lastIndex = amount - 1;

            for (var i = lastIndex; i >= 0; i--)
            {
                Exercise deletedExercise = await exercisesController.DeleteExercise(i);
            }
        }

        [TestMethod]
        public async Task FindExercise_WithExistingId_ReturnsExercise()
        {
            bool found;

            await CleanTable();

            Exercise createdExercise = await exercisesController.CreateExercise(exercise);
            Exercise foundExercise = await exercisesController.FindExercise(0);

            if (foundExercise == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsTrue(found);
            Assert.AreEqual(exercise.Name, createdExercise.Name);
            Assert.AreEqual(exercise.Description, createdExercise.Description);
            Assert.AreEqual(exercise.ImageSource, createdExercise.ImageSource);
        }

        [TestMethod]
        public async Task FindExercise_WithNotExistingId_ReturnsNull()
        {
            bool found;

            await CleanTable();

            Exercise foundExercise = await exercisesController.FindExercise(5);

            if (foundExercise == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsFalse(found);
            Assert.AreEqual(null, foundExercise);
        }
    }
}
