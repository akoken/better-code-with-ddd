using System.Security.Claims;
using LoanApplication.TacticalDdd.DomainModel;
using LoanApplication.TacticalDdd.DomainModel.Ddd;
using LoanApplication.TacticalDdd.DomainModel.DomainEvents;

namespace LoanApplication.TacticalDdd.Application;

public class LoanApplicationDecisionService(IUnitOfWork unitOfWork, ILoanApplicationRepository loanApplications, IOperatorRepository operators, IEventPublisher eventPublisher)
{
    
    public void RejectApplication(string applicationNumber, ClaimsPrincipal principal, string rejectionReason)
    {
        var loanApplication = loanApplications.WithNumber(LoanApplicationNumber.Of(applicationNumber));
        var user = operators.WithLogin(Login.Of(principal.Identity.Name));
            
        loanApplication.Reject(user);
            
        unitOfWork.CommitChanges();
            
        eventPublisher.Publish(new LoanApplicationRejected(loanApplication));
    }

    public void AcceptApplication(string applicationNumber, ClaimsPrincipal principal)
    {
        var loanApplication = loanApplications.WithNumber(LoanApplicationNumber.Of(applicationNumber));
        var user = operators.WithLogin(Login.Of(principal.Identity.Name));
            
        loanApplication.Accept(user);
            
        unitOfWork.CommitChanges();
            
        eventPublisher.Publish(new LoanApplicationAccepted(loanApplication));
    }
}