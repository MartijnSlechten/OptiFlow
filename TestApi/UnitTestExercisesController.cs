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
        Exercise exercise3 = new Exercise(0, "3x in- en uitademen", "adem 3 keer diep in en uit", "imagetest3");

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
            Assert.AreEqual(exercise.Name, foundExercise.Name);
            Assert.AreEqual(exercise.Description, foundExercise.Description);
            Assert.AreEqual(exercise.ImageSource, foundExercise.ImageSource);
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

        [TestMethod]
        public async Task CountExercises_WithZeroExercisesInTable_ReturnsZero()
        {
            await CleanTable();

            int amount = await exercisesController.CountExercises();

            Assert.AreEqual(0, amount);
        }

        [TestMethod]
        public async Task CountExercises_WithOneExerciseInTable_ReturnsOne()
        {
            await CleanTable();

            Exercise createdExercise = await exercisesController.CreateExercise(exercise);
            int amount = await exercisesController.CountExercises();

            Assert.AreEqual(1, amount);
        }

        [TestMethod]
        public async Task CountExercises_WithTwoExercisesInTable_ReturnsTwo()
        {
            await CleanTable();

            Exercise createdExercise = await exercisesController.CreateExercise(exercise);
            Exercise createdExercise2 = await exercisesController.CreateExercise(exercise2);
            int amount = await exercisesController.CountExercises();

            Assert.AreEqual(2, amount);
        }

        [TestMethod]
        public async Task GetLastExercise_WithZeroExercisesInTable_ReturnsNull()
        {
            bool found;

            await CleanTable();

            Exercise foundExercise = await exercisesController.GetLastExercise();

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

        [TestMethod]
        public async Task GetLastExercise_WithOneExerciseInTable_ReturnsExercise()
        {
            bool found;

            await CleanTable();

            Exercise createdExercise = await exercisesController.CreateExercise(exercise);
            Exercise foundExercise = await exercisesController.GetLastExercise();

            if (foundExercise == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsTrue(found);
            Assert.AreEqual(exercise.Name, foundExercise.Name);
            Assert.AreEqual(exercise.Description, foundExercise.Description);
            Assert.AreEqual(exercise.ImageSource, foundExercise.ImageSource);
        }

        [TestMethod]
        public async Task GetLastExercise_WithTwoExercisesInTable_ReturnsSecondExerciseInTable()
        {
            bool found;

            await CleanTable();

            Exercise createdExercise = await exercisesController.CreateExercise(exercise);
            Exercise createdExercise2 = await exercisesController.CreateExercise(exercise2);
            Exercise foundExercise = await exercisesController.GetLastExercise();

            if (foundExercise == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsTrue(found);
            Assert.AreEqual(exercise2.Name, foundExercise.Name);
            Assert.AreEqual(exercise2.Description, foundExercise.Description);
            Assert.AreEqual(exercise2.ImageSource, foundExercise.ImageSource);
        }

        [TestMethod]
        public async Task GetExercises_WithZeroExercisesInTable_ReturnsEmptyList()
        {
            int amount = 0;

            await CleanTable();

            List<Exercise> exercises = await exercisesController.GetExercises();

            foreach (Exercise exercise in exercises)
            {
                amount++;
            }

            Assert.AreEqual(0, amount);
        }

        [TestMethod]
        public async Task GetExercises_WithOneExerciseInTable_ReturnsListWithOneExercise()
        {
            int amount = 0;
            List<Exercise> exercisesInList = new List<Exercise>();

            exercisesInList.Add(exercise);
            await CleanTable();

            Exercise createdExercise = await exercisesController.CreateExercise(exercise);
            List<Exercise> exercises = await exercisesController.GetExercises();

            foreach (Exercise exercise in exercises)
            {
                amount++;
            }

            Assert.AreEqual(1, amount);

            for (int i = 0; i < amount; i++)
            {
                Exercise expectedExercise = exercisesInList[i];
                Exercise actualExercise = exercises[i];

                Assert.AreEqual(expectedExercise.Name, actualExercise.Name);
                Assert.AreEqual(expectedExercise.Description, actualExercise.Description);
                Assert.AreEqual(expectedExercise.ImageSource, actualExercise.ImageSource);
            }
        }

        [TestMethod]
        public async Task GetExercises_WithTwoExercisesInTable_ReturnsListWithTwoExercises()
        {
            int amount = 0;
            List<Exercise> exercisesInList = new List<Exercise>();

            exercisesInList.Add(exercise);
            exercisesInList.Add(exercise2);
            await CleanTable();

            Exercise createdExercise = await exercisesController.CreateExercise(exercise);
            Exercise createdExercise2 = await exercisesController.CreateExercise(exercise2);
            List<Exercise> exercises = await exercisesController.GetExercises();

            foreach (Exercise exercise in exercises)
            {
                amount++;
            }

            Assert.AreEqual(2, amount);

            for (int i = 0; i < amount; i++)
            {
                Exercise expectedExercise = exercisesInList[i];
                Exercise actualExercise = exercises[i];

                Assert.AreEqual(expectedExercise.Name, actualExercise.Name);
                Assert.AreEqual(expectedExercise.Description, actualExercise.Description);
                Assert.AreEqual(expectedExercise.ImageSource, actualExercise.ImageSource);
            }
        }

        [TestMethod]
        public async Task GetExercise_WithExistingId_ReturnsExercise()
        {
            bool found;

            await CleanTable();

            Exercise createdExercise = await exercisesController.CreateExercise(exercise);
            Exercise foundExercise = await exercisesController.GetExercise(0);

            if (foundExercise == null)
            {
                found = false;
            }
            else
            {
                found = true;
            }

            Assert.IsTrue(found);
            Assert.AreEqual(exercise.Name, foundExercise.Name);
            Assert.AreEqual(exercise.Description, foundExercise.Description);
            Assert.AreEqual(exercise.ImageSource, foundExercise.ImageSource);
        }

        [TestMethod]
        public async Task GetExercise_WithNotExistingId_ReturnsNull()
        {
            bool found;

            await CleanTable();

            Exercise foundExercise = await exercisesController.GetExercise(5);

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

        [TestMethod]
        public async Task CreateExercise_ReturnsExercise()
        {
            await CleanTable();

            Exercise createdExercise = await exercisesController.CreateExercise(exercise);

            Assert.AreEqual(exercise.Name, createdExercise.Name);
            Assert.AreEqual(exercise.Description, createdExercise.Description);
            Assert.AreEqual(exercise.ImageSource, createdExercise.ImageSource);
        }

        [TestMethod]
        public async Task UpdateExercise_WithExistingId_ReturnsExercise()
        {
            await CleanTable();

            Exercise createdExercise = await exercisesController.CreateExercise(exercise);
            Exercise updatedExercise = await exercisesController.UpdateExercise(0, exercise3);

            Assert.AreEqual(exercise3.Name, updatedExercise.Name);
            Assert.AreEqual(exercise3.Description, updatedExercise.Description);
            Assert.AreEqual(exercise3.ImageSource, updatedExercise.ImageSource);
        }

        [TestMethod]
        public async Task UpdateExercise_WithNotExistingId_ReturnsNull()
        {
            await CleanTable();

            Exercise createdExercise = await exercisesController.CreateExercise(exercise);
            Exercise updatedExercise = await exercisesController.UpdateExercise(5, exercise3);

            Assert.AreEqual(null, updatedExercise);
        }

        [TestMethod]
        public async Task DeleteExercise_WithExistingId_ReturnsExercise()
        {
            await CleanTable();

            Exercise createdExercise = await exercisesController.CreateExercise(exercise);
            Exercise deletedExercise = await exercisesController.DeleteExercise(0);

            Assert.AreEqual(exercise.Name, deletedExercise.Name);
            Assert.AreEqual(exercise.Description, deletedExercise.Description);
            Assert.AreEqual(exercise.ImageSource, deletedExercise.ImageSource);
        }

        [TestMethod]
        public async Task DeleteExercise_WithNotExistingId_ReturnsNull()
        {
            await CleanTable();

            Exercise createdExercise = await exercisesController.CreateExercise(exercise);
            Exercise deletedExercise = await exercisesController.DeleteExercise(5);

            Assert.AreEqual(null, deletedExercise);
        }
    }
}
