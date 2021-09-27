using NUnit.Framework;

namespace OwlAndJackalope.UX.Tests.Editor
{
    [TestFixture]
    public class DetailObserverTests
    {
        public void InitializeBindsToDetail()
        {
            
        }

        public void InitializeWithInvalidNameBindsOnlyToReference()
        {
            
        }

        public void AddingDetailToReferenceWillInitializeObserverLater()
        {
            
        }

        public void GetValueReturnsValueOfDetail()
        {
            
        }

        public void IsSetReturnsTrueIfDetailIsFound()
        {
            
        }

        public void IsSetReturnsFalseIfDetailIsNotFound()
        {
            
        }

        public void IsSetChangesIfDetailIsUnbound()
        {
            
        }

        public void ChangingDetailFiresOnChangeMethod()
        {
            
        }

        public void ChangingDetailWithSameValueDoesNotFireOnChangeMethod()
        {
            
        }

        public void ChangingReferenceVersionWithNewDetailFiresOnChangeMethod()
        {
            
        }

        public void ChangingReferenceVersionWithSameDetailDoesNotFireOnChangeMethod()
        {
            
        }

        public void SuppressInitialPreventsInitialOnChangeCall()
        {
            
        }

        public void SuppressInitialAsFalseAllowsInitialOnChangeCall()
        {
            
        }

        public void DisposeRemovesReferenceAndDetailListeners()
        {
            
        }
    }
}