namespace Mvp.Model
{
    /// <summary>
    /// Model interface for view - get only data + events
    /// </summary>
    public interface IModelObserver
    {

    }

    /// <summary>
    /// Model interface for controller/presenter
    /// </summary>
    public interface IModel : IModelObserver
    {

    }
    
    public class Model: IModel
    {
     
    }
}