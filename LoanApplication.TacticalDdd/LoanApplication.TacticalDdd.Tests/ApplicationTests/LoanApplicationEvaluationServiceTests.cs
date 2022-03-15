using LoanApplication.TacticalDdd.Application;
using LoanApplication.TacticalDdd.DomainModel;
using LoanApplication.TacticalDdd.Tests.Asserts;
using LoanApplication.TacticalDdd.Tests.Mocks;
using Xunit;
using static LoanApplication.TacticalDdd.Tests.Builders.LoanApplicationBuilder;

namespace LoanApplication.TacticalDdd.Tests.ApplicationTests;

public class LoanApplicationEvaluationServiceTests
{
    [Fact]
    public void LoanApplicationEvaluationService_ApplicationThatSatisfiesAllRules_IsEvaluatedGreen()
    {
        var existingApplications = new InMemoryLoanApplicationRepository(new []
        {
            GivenLoanApplication()
                .WithNumber("123")
                .WithCustomer(customer => customer.WithAge(25).WithIncome(15_000M))
                .WithLoan(loan => loan.WithAmount(200_000).WithNumberOfYears(25).WithInterestRate(1.1M))
                .WithProperty(prop => prop.WithValue(250_000M))
                .Build()
        });
            
        var evaluationService = new LoanApplicationEvaluationService
        (
            new UnitOfWorkMock(),
            existingApplications,
            new DebtorRegistryMock()
        );
            
        evaluationService.EvaluateLoanApplication("123");

        existingApplications.WithNumber(new LoanApplicationNumber("123"))
            .Should()
            .HaveGreenScore();
    }
        
    [Fact]
    public void LoanApplicationEvaluationService_ApplicationThatDoesNotSatisfyAllRules_IsEvaluatedRedAndRejected()
    {
        var existingApplications = new InMemoryLoanApplicationRepository(new []
        {
            GivenLoanApplication()
                .WithNumber("123")
                .WithCustomer(customer => customer.WithAge(55).WithIncome(15_000M))
                .WithLoan(loan => loan.WithAmount(200_000).WithNumberOfYears(25).WithInterestRate(1.1M))
                .WithProperty(prop => prop.WithValue(250_000M))
                .Build()
        });
            
        var evaluationService = new LoanApplicationEvaluationService
        (
            new UnitOfWorkMock(),
            existingApplications,
            new DebtorRegistryMock()
        );
            
        evaluationService.EvaluateLoanApplication("123");

        existingApplications.WithNumber(new LoanApplicationNumber("123"))
            .Should()
            .HaveRedScore()
            .And
            .BeRejected();
    }
}