import { onBeforeUnmount, ref } from "vue";

const useDebouncedFn = <T extends (...args: any[]) => unknown>(fn: T, delayMs = 250) => {
  const timer = ref<number | null>(null);

  onBeforeUnmount(() => {
    if (timer.value !== null) window.clearTimeout(timer.value);
  });

  return (...args: Parameters<T>) => {
    if (timer.value !== null) window.clearTimeout(timer.value);
    timer.value = window.setTimeout(() => {
      void fn(...args);
    }, delayMs);
  };
}

export default useDebouncedFn;
