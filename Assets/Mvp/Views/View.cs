using Fps.MVP.Model;
using Fps.MVP.Presenter;
using UnityEngine;


namespace Fps.MVP.Views
{
     public interface IView
    {
        Transform Transform { get; }
        IModelObserver ModelObserver { get; }
        void AssignPresenter(IPresenter presenter);
    }
    


}