using NUnit.Framework;
using OwlAndJackalope.UX.Runtime.Conditions;
using OwlAndJackalope.UX.Runtime.Conditions.Serialized;
using OwlAndJackalope.UX.Runtime.Data;
using OwlAndJackalope.UX.Runtime.Data.Extensions;
using OwlAndJackalope.UX.Runtime.States;
using OwlAndJackalope.UX.Runtime.States.Serialized;

namespace OwlAndJackalope.UX.Tests.Editor
{
    [TestFixture]
    public class StateTests
    {
        [Test]
        public void StateWithNoConditionsIsNeverActive()
        {
            var detail = new BaseDetail<int>("Detail");
            var state = new BaseRuntimeState("State", new BaseReference(new IDetail[] { detail }), null);
            Assert.That(state.IsActive, Is.False);

            detail.SetValue(2);
            Assert.That(state.IsActive, Is.False);
        }

        [Test]
        public void StateWithSingleConditionIsActiveAtStart()
        {
            var detail = new BaseDetail<int>("Detail");
            var state = new BaseRuntimeState("State", 
                new BaseReference(new IDetail[] { detail }), 
                new ICondition[]
                {
                    new BaseRuntimeCondition<int>(detail.Name, 0, Comparison.Equal)
                });
            Assert.That(state.IsActive, Is.True);
        }

        [Test]
        public void StateWithSingleConditionIsInactiveAtStart()
        {
            var detail = new BaseDetail<int>("Detail");
            var state = new BaseRuntimeState("State", 
                new BaseReference(new IDetail[] { detail }), 
                new ICondition[]
                {
                    new BaseRuntimeCondition<int>(detail.Name, 12, Comparison.Equal)
                });
            Assert.That(state.IsActive, Is.False);
        }

        [Test]
        public void StateWithSingleTrueAndFalseConditionIsActiveAtStart()
        {
            var detail = new BaseDetail<int>("Detail");
            var detail2 = new BaseDetail<int>("Detail2");
            var state = new BaseRuntimeState("State", 
                new BaseReference(new IDetail[] { detail, detail2 }), 
                new ICondition[]
                {
                    new BaseRuntimeCondition<int>(detail.Name, 0, Comparison.Equal),
                    new BaseRuntimeCondition<int>(detail2.Name, 12, Comparison.Equal),
                    
                });
            Assert.That(state.IsActive, Is.True);
        }

        [Test]
        public void StateWithTwoFalseConditionsIsInactiveAtStart()
        {
            var detail = new BaseDetail<int>("Detail");
            var detail2 = new BaseDetail<int>("Detail2");
            var state = new BaseRuntimeState("State", 
                new BaseReference(new IDetail[] { detail, detail2 }), 
                new ICondition[]
                {
                    new BaseRuntimeCondition<int>(detail.Name, 15, Comparison.Equal),
                    new BaseRuntimeCondition<int>(detail2.Name, 12, Comparison.Equal),
                    
                });
            Assert.That(state.IsActive, Is.False);
        }

        [Test]
        public void WhenConditionChangesToTrueStateBecomesActive()
        {
            var detail = new BaseDetail<int>("Detail");
            var detail2 = new BaseDetail<int>("Detail2");
            var state = new BaseRuntimeState("State", 
                new BaseReference(new IDetail[] { detail, detail2 }), 
                new ICondition[]
                {
                    new BaseRuntimeCondition<int>(detail.Name, 15, Comparison.Equal),
                    new BaseRuntimeCondition<int>(detail2.Name, 12, Comparison.Equal),
                    
                });
            Assert.That(state.IsActive, Is.False);
            var changes = 0;
            state.OnStateActiveChanged += () => changes++;
            
            detail.SetValue(15);
            Assert.That(state.IsActive, Is.True);
            Assert.That(changes, Is.EqualTo(1));
        }

        [Test]
        public void WhenConditionChangesToFalseStateBecomesInactive()
        {
            var detail = new BaseDetail<int>("Detail", 15);
            var state = new BaseRuntimeState("State", 
                new BaseReference(new IDetail[] { detail }), 
                new ICondition[]
                {
                    new BaseRuntimeCondition<int>(detail.Name, 15, Comparison.Equal),

                });
            
            Assert.That(state.IsActive, Is.True);
            var changes = 0;
            state.OnStateActiveChanged += () => changes++;
            
            detail.SetValue(9);
            Assert.That(state.IsActive, Is.False);
            Assert.That(changes, Is.EqualTo(1));
        }

        [Test]
        public void IfOneConditionChangesToFalseAndOtherOneRemainsTrueStateRemainsActive()
        {
            var detail = new BaseDetail<int>("Detail", 15);
            var detail2 = new BaseDetail<int>("Detail2", 12);
            var state = new BaseRuntimeState("State", 
                new BaseReference(new IDetail[] { detail, detail2 }), 
                new ICondition[]
                {
                    new BaseRuntimeCondition<int>(detail.Name, 15, Comparison.Equal),
                    new BaseRuntimeCondition<int>(detail2.Name, 12, Comparison.Equal),
                    
                });
            
            Assert.That(state.IsActive, Is.True);
            var changes = 0;
            state.OnStateActiveChanged += () => changes++;
            
            detail.SetValue(9);
            Assert.That(state.IsActive, Is.True);
            Assert.That(changes, Is.EqualTo(0));
        }

        [Test]
        public void WhenConditionChangesToSameStateRemainsSame()
        {
            var detail = new BaseDetail<int>("Detail", 15);
            var state = new BaseRuntimeState("State", 
                new BaseReference(new IDetail[] { detail }), 
                new ICondition[]
                {
                    new BaseRuntimeCondition<int>(detail.Name, 15, Comparison.GreaterThanEqual),
                });
            
            Assert.That(state.IsActive, Is.True);
            var changes = 0;
            state.OnStateActiveChanged += () => changes++;
            
            detail.SetValue(16);
            Assert.That(state.IsActive, Is.True);
            Assert.That(changes, Is.EqualTo(0));
        }

        [Test]
        public void WhenReferenceChangesConditionsStateResponds()
        {
            var detail = new BaseDetail<int>("Detail", 15);
            var reference = new BaseReference(new IDetail[] { detail });
            var state = new BaseRuntimeState("State", 
                reference, 
                new ICondition[]
                {
                    new BaseRuntimeCondition<int>(detail.Name, 15, Comparison.GreaterThanEqual),
                });
            
            Assert.That(state.IsActive, Is.True);
            var changes = 0;
            state.OnStateActiveChanged += () => changes++;

            reference.AddDetail(new BaseDetail<int>("Detail", 0));
            Assert.That(state.IsActive, Is.False);
            Assert.That(changes, Is.EqualTo(1));
        }

        [Test]
        public void WhenReferenceChangesOtherConditionsStateDoesNothing()
        {
            var detail = new BaseDetail<int>("Detail", 15);
            var detail2 = new BaseDetail<int>("Detail2", 15);
            var reference = new BaseReference(new IDetail[] { detail, detail2 });
            var state = new BaseRuntimeState("State", 
                reference, 
                new ICondition[]
                {
                    new BaseRuntimeCondition<int>(detail.Name, 15, Comparison.GreaterThanEqual),
                });
            
            Assert.That(state.IsActive, Is.True);
            var changes = 0;
            state.OnStateActiveChanged += () => changes++;

            reference.AddDetail(new BaseDetail<int>("Detail2", 0));
            Assert.That(state.IsActive, Is.True);
            Assert.That(changes, Is.EqualTo(0));
        }

        [Test]
        public void ConvertCreatesConditions()
        {
            var serializedState = new BaseSerializedState("State", new SerializedConditionAndGroup[]
            {
                new SerializedConditionAndGroup(new BaseSerializedCondition[]
                {
                    BaseSerializedCondition.Create<int>("Detail", 15, Comparison.Equal), 
                })
            });

            var reference = new BaseReference(new IDetail[] { new BaseDetail<int>("Detail") });
            var state = serializedState.ConvertToState(reference);
            
            Assert.That(state.Name, Is.EqualTo("State"));
            Assert.That(state.IsActive, Is.False);

            reference.GetMutable<int>("Detail").SetValue(15);
            Assert.That(state.IsActive, Is.True);
        }
    }
}