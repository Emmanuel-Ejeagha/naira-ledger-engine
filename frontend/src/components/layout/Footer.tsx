import { Link } from 'react-router-dom';

export default function Footer() {
  return (
    <footer className="border-t border-border bg-card mt-auto">
      <div className="max-w-7xl mx-auto px-6 py-6 flex flex-col md:flex-row items-center justify-between gap-4 text-sm text-muted-foreground">
        <div className="flex items-center gap-4">
          <Link to="/" className="hover:text-foreground transition-colors">Home</Link>
          <a href="#" className="hover:text-foreground transition-colors">Privacy</a>
          <a href="#" className="hover:text-foreground transition-colors">Terms</a>
        </div>
        <div className="flex items-center gap-4">
          <a href="#" className="hover:text-foreground transition-colors" aria-label="Twitter">𝕏</a>
          <a href="#" className="hover:text-foreground transition-colors" aria-label="LinkedIn">In</a>
          <a href="#" className="hover:text-foreground transition-colors" aria-label="GitHub">GitHub</a>
        </div>
        <p>&copy; {new Date().getFullYear()} NairaLedger. All rights reserved.</p>
      </div>
    </footer>
  );
}