using OWP.SumReport.LAReport.Models;

namespace OWP.SumReport.LAReport.Services
{
    public interface SumUpService
    {
        void Execute(IList<Assignment> assignments, LaReportModel report);
    }

    public class SumUpServiceImp : SumUpService
    {
        private readonly SpecialAndMixedAndESoccerService specialAndMixedAndESoccerService;

        private readonly SingleProductService singleProductService;

        private readonly MultipleProductService multipleProductService;

        public SumUpServiceImp(SpecialAndMixedAndESoccerService specialAndMixedAndESoccerService, SingleProductService singleProductService, MultipleProductService multipleProductService)
        {
            this.specialAndMixedAndESoccerService = specialAndMixedAndESoccerService;
            this.singleProductService = singleProductService;
            this.multipleProductService = multipleProductService;
        }

        public void Execute(IList<Assignment> assignments, LaReportModel report)
        {
            specialAndMixedAndESoccerService.SumUp(assignments,report);
            singleProductService.SumUp(assignments,report);
            multipleProductService.SumUp(assignments,report);
        }
    }
}
