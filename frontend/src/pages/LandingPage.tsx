import { Link } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Shield, Zap, Bell, QrCode, ArrowRightLeft, BadgeCheck, Users, Star } from 'lucide-react';

const features = [
  { icon: ArrowRightLeft, title: 'Double‑Entry Ledger', desc: 'Every transaction is immutable and auditable with mandatory debit‑credit balancing.' },
  { icon: Bell, title: 'Real‑time Notifications', desc: 'Receive instant alerts for transfers, funding, KYC updates, and more.' },
  { icon: Shield, title: 'Bank‑Grade Security', desc: 'JWT authentication, encrypted data, and fraud velocity checks keep your money safe.' },
  { icon: BadgeCheck, title: 'KYC Verification', desc: 'Tiered KYC levels with document upload, admin approval, and compliance tracking.' },
  { icon: Zap, title: 'Instant Transfers', desc: 'Send NGN to any NairaLedger wallet instantly with idempotency guarantees.' },
  { icon: QrCode, title: 'QR Payments', desc: 'Generate and share QR codes to receive payments effortlessly.' },
];

export default function LandingPage() {
  return (
    <>
      {/* Hero */}
      <section className="max-w-7xl mx-auto px-6 py-20 text-center">
        <h1 className="text-5xl md:text-6xl font-bold tracking-tight">
          The modern wallet for{' '}
          <span className="text-success">Nigerian payments</span>
        </h1>
        <p className="mt-6 text-lg text-muted-foreground max-w-2xl mx-auto">
          NairaLedger combines a double‑entry ledger, real‑time fraud protection, and seamless Paystack funding to give you a secure and transparent digital wallet experience.
        </p>
        <div className="mt-10 flex items-center justify-center gap-4">
          <Link to="/register">
            <Button size="lg" className="text-base">Create Free Account</Button>
          </Link>
          <Link to="/login">
            <Button variant="outline" size="lg" className="text-base">Sign In</Button>
          </Link>
        </div>
      </section>

      {/* Features */}
      <section className="bg-muted/50 py-20">
        <div className="max-w-7xl mx-auto px-6">
          <h2 className="text-3xl font-bold text-center">Everything you need in a wallet</h2>
          <div className="mt-12 grid gap-8 md:grid-cols-2 lg:grid-cols-3">
            {features.map(({ icon: Icon, title, desc }) => (
              <div key={title} className="bg-card border border-border rounded-lg p-6">
                <Icon className="h-8 w-8 text-success mb-4" />
                <h3 className="font-semibold text-lg">{title}</h3>
                <p className="mt-2 text-sm text-muted-foreground">{desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Security */}
      <section className="max-w-7xl mx-auto px-6 py-20 text-center">
        <Shield className="h-12 w-12 mx-auto text-success" />
        <h2 className="mt-4 text-3xl font-bold">Your security is our priority</h2>
        <p className="mt-4 text-muted-foreground max-w-2xl mx-auto">
          Every transaction is encrypted, every action is logged, and our fraud engine monitors activity 24/7. Your funds are protected by the same standards used by leading financial institutions.
        </p>
      </section>

      {/* Real‑time notifications */}
      <section className="bg-muted/50 py-20">
        <div className="max-w-7xl mx-auto px-6 text-center">
          <Bell className="h-12 w-12 mx-auto text-success" />
          <h2 className="mt-4 text-3xl font-bold">Never miss a beat</h2>
          <p className="mt-4 text-muted-foreground max-w-2xl mx-auto">
            Receive instant pop‑up notifications and emails for transfers, funding, KYC changes, and security alerts.
          </p>
        </div>
      </section>

      {/* QR Payments */}
      <section className="max-w-7xl mx-auto px-6 py-20 text-center">
        <QrCode className="h-12 w-12 mx-auto text-success" />
        <h2 className="mt-4 text-3xl font-bold">Accept payments with QR</h2>
        <p className="mt-4 text-muted-foreground max-w-2xl mx-auto">
          Generate a unique QR code for your wallet and let customers pay you instantly — no more typing long account numbers.
        </p>
      </section>

      {/* Testimonials */}
      <section className="bg-muted/50 py-20">
        <div className="max-w-7xl mx-auto px-6 text-center">
          <h2 className="text-3xl font-bold">Trusted by users</h2>
          <div className="mt-12 grid gap-8 md:grid-cols-3">
            {[1,2,3].map((i) => (
              <div key={i} className="bg-card border border-border rounded-lg p-6">
                <Users className="h-8 w-8 mx-auto text-success mb-4" />
                <Star className="h-4 w-4 mx-auto text-yellow-500 mb-2" />
                <Star className="h-4 w-4 mx-auto text-yellow-500 mb-2" />
                <p className="text-sm text-muted-foreground">"NairaLedger makes transfers so easy. The real‑time notifications are a game changer."</p>
                <p className="mt-4 font-semibold">- Happy User</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA */}
      <section className="max-w-7xl mx-auto px-6 py-20 text-center">
        <h2 className="text-3xl font-bold">Ready to take control of your finances?</h2>
        <p className="mt-4 text-muted-foreground">Join thousands of users who trust NairaLedger for secure, fast, and transparent payments.</p>
        <div className="mt-8">
          <Link to="/register">
            <Button size="lg" className="text-base">Open Your Free Account</Button>
          </Link>
        </div>
      </section>
    </>
  );
}