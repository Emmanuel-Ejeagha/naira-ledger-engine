import { type ReactNode, useState } from 'react';

interface Props {
  content: string;
  children: ReactNode;
}

export default function Tooltip({ content, children }: Props) {
  const [visible, setVisible] = useState(false);
  return (
    <div
      className="relative block"   // ← changed from inline-block to block
      onMouseEnter={() => setVisible(true)}
      onMouseLeave={() => setVisible(false)}
    >
      {children}
      {visible && (
        <div className="absolute left-full top-1/2 -translate-y-1/2 ml-2 px-2 py-1 bg-foreground text-background text-xs rounded whitespace-nowrap z-50">
          {content}
        </div>
      )}
    </div>
  );
}