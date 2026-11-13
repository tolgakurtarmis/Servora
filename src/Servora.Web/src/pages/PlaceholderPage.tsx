type PlaceholderPageProps = {
  title: string;
  description?: string;
};

export function PlaceholderPage({
  title,
  description = 'Bu sayfa yakında hazır olacak.',
}: PlaceholderPageProps) {
  return (
    <div>
      <h1>{title}</h1>
      <p className="muted">{description}</p>
    </div>
  );
}
