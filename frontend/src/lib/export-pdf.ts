import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import type { TransactionDto } from '../types/transaction';

export function downloadPDF(transactions: TransactionDto[], walletId: string) {
  const doc = new jsPDF();

  // Title
  doc.setFontSize(18);
  doc.text('NairaLedger - Transaction Statement', 14, 22);

  doc.setFontSize(10);
  doc.text(`Wallet: ${walletId}`, 14, 30);
  doc.text(`Date: ${new Date().toLocaleDateString()}`, 14, 36);

  const headers = [['Reference', 'Type', 'Amount (NGN)', 'Status', 'Date']];
  const data = transactions.map(tx => [
    tx.reference,
    tx.type,
    tx.amount.toFixed(2),
    tx.status,
    new Date(tx.createdAt).toLocaleDateString(),
  ]);

  autoTable(doc, {
    head: headers,
    body: data,
    startY: 42,
    styles: { fontSize: 8 },
    headStyles: { fillColor: [15, 23, 42] }, // primary color
  });

  doc.save(`statement-${walletId.slice(0, 8)}.pdf`);
}