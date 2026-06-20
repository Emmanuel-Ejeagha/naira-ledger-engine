import { getPasswordStrength } from '@/lib/passwordStrength';

const colors: Record<string, string> = {
  'Very Weak': 'bg-red-500',
  Weak: 'bg-orange-500',
  Fair: 'bg-yellow-500',
  Strong: 'bg-lime-500',
  'Very Strong': 'bg-green-600',
};

interface Props {
  password: string;
}

export default function PasswordStrengthMeter({ password }: Props) {
  if (!password) return null;
  const { level } = getPasswordStrength(password);
  return (
    <div className="mt-1 space-y-1">
      <div className="h-1 w-full rounded bg-muted">
        <div
          className={`h-1 rounded transition-all duration-300 ${colors[level]}`}
          style={{ width: `${(100 / 5) * ['Very Weak','Weak','Fair','Strong','Very Strong'].indexOf(level) + 1}%` }}
        />
      </div>
      <p className="text-xs text-muted-foreground">{level}</p>
    </div>
  );
}