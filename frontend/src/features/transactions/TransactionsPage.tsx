import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { getTransactions, type TransactionFilters } from '@/api/transactions';
import type { TransactionDto } from '@/types/transaction';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Card, CardContent } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Download, Search, ChevronLeft, ChevronRight, FileText } from 'lucide-react';
import { toast } from 'sonner';
import { useAuthStore } from '@/stores/authStore';
import { downloadCSV } from '@/lib/export-csv';
import { downloadPDF } from '@/lib/export-pdf';

const PAGE_SIZE = 10;

export default function TransactionsPage() {
  const walletId = useAuthStore((s) => s.walletId);
  const [cursor, setCursor] = useState<string | undefined>(undefined);
  const [history, setHistory] = useState<TransactionDto[][]>([]);

  // Filters
  const [search, setSearch] = useState('');
  const [type, setType] = useState<string>('all');
  const [dateFrom, setDateFrom] = useState('');
  const [dateTo, setDateTo] = useState('');

  const filters: TransactionFilters = {
    walletId,
    pageSize: PAGE_SIZE,
    cursor,
    search: search || undefined,
    type: type !== 'all' ? type : undefined,
    dateFrom: dateFrom || undefined,
    dateTo: dateTo || undefined,
  };

  const { data, isLoading, isError, error, isFetching } = useQuery({
    queryKey: ['transactions', walletId, cursor, search, type, dateFrom, dateTo],
    queryFn: () => getTransactions(filters),
    enabled: !!walletId, 
    keepPreviousData: true,
    onSuccess: (newData) => {
      setHistory((prev) => {
        const newHistory = [...prev];
        if (cursor) {
          const existingIndex = newHistory.findIndex(
            (page) => page[0]?.transactionId === newData.items[0]?.transactionId
          );
          if (existingIndex >= 0) {
            newHistory[existingIndex] = newData.items;
          } else {
            newHistory.push(newData.items);
          }
        } else {
          return [newData.items];
        }
        return newHistory;
      });
    },
    onError: () => {
      toast.error('Failed to load transactions');
    },
  });

  const hasNextPage = data?.hasMore ?? false;
  const currentPageItems = data?.items ?? [];
  const isEmpty = !isLoading && currentPageItems.length === 0;

  const handleNextPage = () => {
    if (hasNextPage && data?.nextCursor) {
      setCursor(data.nextCursor);
    }
  };

  const handlePrevPage = () => {
    setCursor(undefined);
    setHistory([]);
  };

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearch(e.target.value);
    setCursor(undefined);
    setHistory([]);
  };

  const handleTypeChange = (value: string) => {
    setType(value);
    setCursor(undefined);
    setHistory([]);
  };

  const handleDateChange = (field: 'dateFrom' | 'dateTo', value: string) => {
    if (field === 'dateFrom') setDateFrom(value);
    else setDateTo(value);
    setCursor(undefined);
    setHistory([]);
  };

  const handleExportCSV = () => {
    if (currentPageItems.length === 0) {
      toast.error('No transactions to export');
      return;
    }
    downloadCSV(currentPageItems);
    toast.success('CSV downloaded');
  };

  const handleExportPDF = () => {
    if (currentPageItems.length === 0) {
      toast.error('No transactions to export');
      return;
    }
    downloadPDF(currentPageItems, walletId!);
    toast.success('PDF downloaded');
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Transaction History</h1>
        <div className="flex gap-2">
          <Button variant="outline" size="sm" onClick={handleExportCSV}>
            <Download className="h-4 w-4 mr-2" /> CSV
          </Button>
          <Button variant="outline" size="sm" onClick={handleExportPDF}>
            <FileText className="h-4 w-4 mr-2" /> PDF
          </Button>
        </div>
      </div>

      {/* Filters */}
      <Card>
        <CardContent className="pt-6">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div className="space-y-2">
              <Label htmlFor="search">Search reference</Label>
              <div className="relative">
                <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
                <Input
                  id="search"
                  placeholder="Reference..."
                  className="pl-8"
                  value={search}
                  onChange={handleSearchChange}
                />
              </div>
            </div>
            <div className="space-y-2">
              <Label>Type</Label>
              <Select value={type} onValueChange={handleTypeChange}>
                <SelectTrigger>
                  <SelectValue placeholder="All types" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All types</SelectItem>
                  <SelectItem value="Funding">Funding</SelectItem>
                  <SelectItem value="Transfer">Transfer</SelectItem>
                  <SelectItem value="Reversal">Reversal</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="dateFrom">From</Label>
              <Input
                id="dateFrom"
                type="date"
                value={dateFrom}
                onChange={(e) => handleDateChange('dateFrom', e.target.value)}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="dateTo">To</Label>
              <Input
                id="dateTo"
                type="date"
                value={dateTo}
                onChange={(e) => handleDateChange('dateTo', e.target.value)}
              />
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Results */}
      {isLoading && (
        <div className="space-y-2">
          {[...Array(5)].map((_, i) => (
            <Skeleton key={i} className="h-10 w-full" />
          ))}
        </div>
      )}

      {isError && (
        <Alert variant="destructive">
          <AlertDescription>Error loading transactions: {error?.message}</AlertDescription>
        </Alert>
      )}

      {isEmpty && (
        <div className="text-center py-12">
          <p className="text-muted-foreground">No transactions found.</p>
        </div>
      )}

      {!isEmpty && !isLoading && (
        <>
          <Card>
            <CardContent className="p-0">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b border-border text-left text-muted-foreground">
                    <th className="py-3 px-4">Reference</th>
                    <th className="py-3 px-4">Type</th>
                    <th className="py-3 px-4">Amount</th>
                    <th className="py-3 px-4">Status</th>
                    <th className="py-3 px-4">Date</th>
                  </tr>
                </thead>
                <tbody>
                  {currentPageItems.map((tx) => (
                    <tr key={tx.transactionId} className="border-b border-border hover:bg-muted/50">
                      <td className="py-3 px-4 font-mono text-xs">{tx.reference}</td>
                      <td className="py-3 px-4">{tx.type}</td>
                      <td className="py-3 px-4 font-semibold">
                        <span className={tx.amount > 0 ? 'text-success' : 'text-destructive'}>
                          NGN {Math.abs(tx.amount).toFixed(2)}
                        </span>
                      </td>
                      <td className="py-3 px-4">
                        <span
                          className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${
                            tx.status === 'Completed' ? 'bg-success/10 text-success' : 'bg-warning/10 text-warning'
                          }`}
                        >
                          {tx.status}
                        </span>
                      </td>
                      <td className="py-3 px-4 text-muted-foreground">{new Date(tx.createdAt).toLocaleDateString()}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </CardContent>
          </Card>

          {/* Pagination */}
          <div className="flex items-center justify-between">
            <Button
              variant="outline"
              size="sm"
              onClick={handlePrevPage}
              disabled={history.length <= 1 && !cursor}
            >
              <ChevronLeft className="h-4 w-4 mr-1" /> Previous
            </Button>
            <span className="text-sm text-muted-foreground">
              Page {history.length} {isFetching && '(loading...)'}
            </span>
            <Button
              variant="outline"
              size="sm"
              onClick={handleNextPage}
              disabled={!hasNextPage || isFetching}
            >
              Next <ChevronRight className="h-4 w-4 ml-1" />
            </Button>
          </div>
        </>
      )}
    </div>
  );
}