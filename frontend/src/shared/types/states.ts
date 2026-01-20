
export type EmptyStatePropsProps = {
  title?: string;
  message?: string;
  actionLabel?: string;
  onAction?: () => void;
  testId?: string;
};

export type ErrorStateProps = {
  title?: string;
  message: string;
  onRetry?: () => void;
  retryLabel?: string;
  testId?: string;
};

export type LoadingStateProps = {
  title?: string;
  description?: string;
  testId?: string;
};
