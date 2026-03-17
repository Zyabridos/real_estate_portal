<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from "vue";

type Props = {
  images: string[];
  intervalMs?: number;
};

const props = withDefaults(defineProps<Props>(), {
  intervalMs: 5000,
});

const activeIndex = ref(0);
let autoplayId: number | null = null;

const safeImages = computed(() =>
  (props.images ?? []).filter((image): image is string => Boolean(image?.trim())),
);

const hasMultipleImages = computed(() => safeImages.value.length > 1);

function stopAutoplay(): void {
  if (autoplayId !== null) {
    window.clearInterval(autoplayId);
    autoplayId = null;
  }
}

function goTo(index: number): void {
  if (!safeImages.value.length) {
    activeIndex.value = 0;
    return;
  }

  const normalizedIndex =
    ((index % safeImages.value.length) + safeImages.value.length) %
    safeImages.value.length;

  activeIndex.value = normalizedIndex;
}

function goNext(): void {
  if (!hasMultipleImages.value) return;
  goTo(activeIndex.value + 1);
}

function startAutoplay(): void {
  stopAutoplay();

  if (!hasMultipleImages.value) return;

  autoplayId = window.setInterval(() => {
    goNext();
  }, props.intervalMs);
}

watch(
  safeImages,
  (nextImages) => {
    if (!nextImages.length) {
      activeIndex.value = 0;
      stopAutoplay();
      return;
    }

    if (activeIndex.value > nextImages.length - 1) {
      activeIndex.value = 0;
    }

    startAutoplay();
  },
  { immediate: true },
);

onMounted(() => {
  startAutoplay();
});

onBeforeUnmount(() => {
  stopAutoplay();
});
</script>

<template>
  <div class="absolute inset-0 overflow-hidden">
    <div
      v-for="(image, index) in safeImages"
      :key="`${image}-${index}`"
      class="absolute inset-0 transition-opacity duration-1000 ease-in-out"
      :class="index === activeIndex ? 'opacity-100' : 'opacity-0'"
    >
      <img
        :src="image"
        :alt="`Background image ${index + 1}`"
        class="h-full w-full object-cover"
      />
    </div>

    <div
      class="pointer-events-none absolute inset-0 bg-[radial-gradient(70%_60%_at_18%_14%,rgba(255,255,255,0.18),transparent_55%),radial-gradient(60%_55%_at_82%_4%,rgba(255,255,255,0.12),transparent_55%)]"
      aria-hidden="true"
    />

    <div
      class="pointer-events-none absolute inset-0 bg-gradient-to-br from-slate-950/70 via-slate-900/45 to-slate-950/75"
      aria-hidden="true"
    />

    <div
      v-if="hasMultipleImages"
      class="absolute bottom-5 left-1/2 z-10 flex -translate-x-1/2 items-center gap-2"
    >
      <button
        v-for="(_, index) in safeImages"
        :key="index"
        type="button"
        class="h-2.5 w-2.5 rounded-full transition"
        :class="
          index === activeIndex
            ? 'bg-white shadow-[0_0_0_4px_rgba(255,255,255,0.18)]'
            : 'bg-white/45 hover:bg-white/70'
        "
        :aria-label="`Go to image ${index + 1}`"
        @click="goTo(index)"
      />
    </div>
  </div>
</template>
