using Fps.MVP.Model;
using Fps.MVP.Views;

namespace Fps.MVP.Presenter
{
    public interface IPresenter
    {
        
    }
    
    
    public abstract class Presenter<TModel>:IPresenter
        where TModel : IModel
    {
        protected TModel Model { get; }
        protected Presenter(TModel model)
        {
            Model = model;
        }
    }
    
    public abstract class Presenter<TModel,TView>:Presenter<TModel>
        where TModel : IModel
        where TView : IView
    {
        protected TView View { get; }
        protected Presenter(TModel model, TView view) : base(model)
        {
            View = view;
            view.AssignPresenter(this);
        }
    }
}