using Scripts.Features.Grid;
using Zenject;

namespace Initialization
{
    public class GameInitializer
    {
        [Inject] private GridService _gridService;
        
        [Inject]
        public void Init()
        {
            _gridService.SetupGrid();
        }
        
    }
}