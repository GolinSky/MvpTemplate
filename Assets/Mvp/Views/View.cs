using Mvp.Model;
using Mvp.Presenter;
using UnityEngine;

namespace Mvp.Views
{
     public interface IView
    {
        Transform Transform { get; }
        IModelObserver ModelObserver { get; }
        void AssignPresenter(IPresenter presenter);
    }
    


}