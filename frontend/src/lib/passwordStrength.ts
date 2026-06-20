export type StrengthLevel = 'Very Weak' | 'Weak' | 'Fair' | 'Strong' | 'Very Strong';

export function getPasswordStrength(password: string): { level: StrengthLevel; score: number } {
  let score = 0;
  if (password.length >= 6) score++;
  if (password.length >= 8) score++;
  if (password.length >= 12) score++;
  if (password.length >= 16) score++;

  let variety = 0;
  if (/[a-z]/.test(password)) variety++;
  if (/[A-Z]/.test(password)) variety++;
  if (/[0-9]/.test(password)) variety++;
  if (/[^a-zA-Z0-9]/.test(password)) variety++;
  score += variety;

  const level: StrengthLevel =
    score <= 2 ? 'Very Weak' :
    score <= 4 ? 'Weak' :
    score <= 6 ? 'Fair' :
    score <= 7 ? 'Strong' : 'Very Strong';

  return { level, score };
}