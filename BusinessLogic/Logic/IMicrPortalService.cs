using Domain.ViewModels.AccountTypes;
using Domain.ViewModels.ApprovalStatuses;
using Domain.ViewModels.BankBranches;
using Domain.ViewModels.Banks;
using Domain.ViewModels.BookTypes;
using Domain.ViewModels.Currencies;
using Domain.ViewModels.NumberOfLeaflets;
using Domain.ViewModels.Regions;
using Domain.ViewModels.Statuses;
using Domain.ViewModels.TransactionCodes;

namespace BusinessLogic.Logic;

/// <summary>
/// Central portal interface for all MICR Cheque Processing System business logic.
/// All controllers and external consumers should depend ONLY on this interface.
/// </summary>
public interface IMicrPortalService
{
    #region Bank Operations
    Task<List<BankDto>> GetBanksAsync(CancellationToken ct = default);
    Task<BankDto?> GetBankByIdAsync(long bankId, CancellationToken ct = default);
    Task<BankDto> CreateBankAsync(BankFormViewModel form, CancellationToken ct = default);
    Task<BankDto> UpdateBankAsync(long bankId, BankFormViewModel form, CancellationToken ct = default);
    Task<bool> DeleteBankAsync(long bankId, CancellationToken ct = default);
    #endregion

    #region Bank Branch Operations
    Task<List<BankBranchDto>> GetBankBranchesAsync(CancellationToken ct = default);
    Task<List<BankBranchDto>> GetBankBranchesByBankIdAsync(long bankId, CancellationToken ct = default);
    Task<BankBranchDto?> GetBankBranchByIdAsync(long bankBranchId, CancellationToken ct = default);
    Task<BankBranchDto> CreateBankBranchAsync(BankBranchFormViewModel form, CancellationToken ct = default);
    Task<BankBranchDto> UpdateBankBranchAsync(long bankBranchId, BankBranchFormViewModel form, CancellationToken ct = default);
    Task<bool> DeleteBankBranchAsync(long bankBranchId, CancellationToken ct = default);
    #endregion

    #region Account Type Operations
    Task<List<AccountTypeDto>> GetAccountTypesAsync(CancellationToken ct = default);
    Task<AccountTypeDto?> GetAccountTypeByIdAsync(long accountTypeId, CancellationToken ct = default);
    Task<AccountTypeDto> CreateAccountTypeAsync(AccountTypeFormViewModel form, CancellationToken ct = default);
    Task<AccountTypeDto> UpdateAccountTypeAsync(long accountTypeId, AccountTypeFormViewModel form, CancellationToken ct = default);
    Task<bool> DeleteAccountTypeAsync(long accountTypeId, CancellationToken ct = default);
    #endregion

    #region Region Operations
    Task<List<RegionDto>> GetRegionsAsync(CancellationToken ct = default);
    Task<RegionDto?> GetRegionByIdAsync(long regionId, CancellationToken ct = default);
    Task<RegionDto> CreateRegionAsync(RegionFormViewModel form, CancellationToken ct = default);
    Task<RegionDto> UpdateRegionAsync(long regionId, RegionFormViewModel form, CancellationToken ct = default);
    Task<bool> DeleteRegionAsync(long regionId, CancellationToken ct = default);
    #endregion

    #region Currency Operations
    Task<List<CurrencyDto>> GetCurrenciesAsync(CancellationToken ct = default);
    Task<CurrencyDto?> GetCurrencyByIdAsync(long currencyId, CancellationToken ct = default);
    Task<CurrencyDto> CreateCurrencyAsync(CurrencyFormViewModel form, CancellationToken ct = default);
    Task<CurrencyDto> UpdateCurrencyAsync(long currencyId, CurrencyFormViewModel form, CancellationToken ct = default);
    Task<bool> DeleteCurrencyAsync(long currencyId, CancellationToken ct = default);
    #endregion

    #region Status Operations
    Task<List<StatusDto>> GetStatusesAsync(CancellationToken ct = default);
    Task<StatusDto?> GetStatusByIdAsync(long statusId, CancellationToken ct = default);
    Task<StatusDto> CreateStatusAsync(StatusFormViewModel form, CancellationToken ct = default);
    Task<StatusDto> UpdateStatusAsync(long statusId, StatusFormViewModel form, CancellationToken ct = default);
    Task<bool> DeleteStatusAsync(long statusId, CancellationToken ct = default);
    #endregion

    #region Transaction Code Operations
    Task<List<TransactionCodeDto>> GetTransactionCodesAsync(CancellationToken ct = default);
    Task<TransactionCodeDto?> GetTransactionCodeByIdAsync(long transactionCodeId, CancellationToken ct = default);
    Task<TransactionCodeDto> CreateTransactionCodeAsync(TransactionCodeFormViewModel form, CancellationToken ct = default);
    Task<TransactionCodeDto> UpdateTransactionCodeAsync(long transactionCodeId, TransactionCodeFormViewModel form, CancellationToken ct = default);
    Task<bool> DeleteTransactionCodeAsync(long transactionCodeId, CancellationToken ct = default);
    #endregion

    #region Book Type Operations
    Task<List<BookTypeDto>> GetBookTypesAsync(CancellationToken ct = default);
    Task<BookTypeDto?> GetBookTypeByIdAsync(long bookTypeId, CancellationToken ct = default);
    Task<BookTypeDto> CreateBookTypeAsync(BookTypeFormViewModel form, CancellationToken ct = default);
    Task<BookTypeDto> UpdateBookTypeAsync(long bookTypeId, BookTypeFormViewModel form, CancellationToken ct = default);
    Task<bool> DeleteBookTypeAsync(long bookTypeId, CancellationToken ct = default);
    #endregion

    #region Number Of Leaflet Operations
    Task<List<NumberOfLeafletDto>> GetNumberOfLeafletsAsync(CancellationToken ct = default);
    Task<NumberOfLeafletDto?> GetNumberOfLeafletByIdAsync(long numberOfLeafletId, CancellationToken ct = default);
    Task<NumberOfLeafletDto> CreateNumberOfLeafletAsync(NumberOfLeafletFormViewModel form, CancellationToken ct = default);
    Task<NumberOfLeafletDto> UpdateNumberOfLeafletAsync(long numberOfLeafletId, NumberOfLeafletFormViewModel form, CancellationToken ct = default);
    Task<bool> DeleteNumberOfLeafletAsync(long numberOfLeafletId, CancellationToken ct = default);
    #endregion

    #region Approval Status Operations
    Task<List<ApprovalStatusDto>> GetApprovalStatusesAsync(CancellationToken ct = default);
    Task<ApprovalStatusDto?> GetApprovalStatusByIdAsync(long approvalStatusId, CancellationToken ct = default);
    Task<ApprovalStatusDto> CreateApprovalStatusAsync(ApprovalStatusFormViewModel form, CancellationToken ct = default);
    Task<ApprovalStatusDto> UpdateApprovalStatusAsync(long approvalStatusId, ApprovalStatusFormViewModel form, CancellationToken ct = default);
    Task<bool> DeleteApprovalStatusAsync(long approvalStatusId, CancellationToken ct = default);
    #endregion

    #region Cheque Operations (Placeholders for future implementation)
    // Task<List<ChequeDto>> GetChequesAsync(CancellationToken ct = default);
    // Task<ChequeDto> CreateChequeAsync(ChequeFormViewModel form, CancellationToken ct = default);
    // Task<ChequeDto> VerifyChequeAsync(long chequeId, CancellationToken ct = default);
    // Task<ChequeDto> ApproveChequeAsync(long chequeId, long approvedByUserId, CancellationToken ct = default);
    // Task<ChequeDto> ProcessChequePaymentAsync(long chequeId, CancellationToken ct = default);
    // Task<ChequeDto> RejectChequeAsync(long chequeId, string reason, CancellationToken ct = default);
    #endregion

    #region Reporting (Placeholders for future implementation)
    // Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken ct = default);
    // Task<List<ChequeReportDto>> GetChequeReportAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default);
    // Task<List<TransactionReportDto>> GetTransactionReportAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default);
    #endregion

    #region User Context
    long GetCurrentUserId();
    #endregion
}
