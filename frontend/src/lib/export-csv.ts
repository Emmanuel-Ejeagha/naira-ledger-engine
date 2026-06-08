import type { TransactionDto } from "../types/transaction";

export function downloadCSV(transactions: TransactionDto[], filename = 'transactions.csv') {
  const headers = ['Reference', 'Type', 'Amount', 'Status', 'Date'];
  const rows = transactions.map(tx => [
    tx.reference,
    tx.type,
    tx.amount.toFixed(2),
    tx.status,
    new Date(tx.createdAt).toLocaleDateString(),
  ]);

  const csvContent = [headers, ...rows]
    .map(row => row.map(cell => `"${cell}"`).join(','))
    .join('\n');

  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
  const url = URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.download = filename;
  link.click();
  URL.revokeObjectURL(url);
}