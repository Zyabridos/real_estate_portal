<script setup lang="ts">
import { computed, ref, watch } from "vue";

type GalleryImage = {
  src: string;
  alt: string;
};

type GalleryBadge = {
  label: string;
  class?: string;
  testId?: string;
};

type Props = {
  images: GalleryImage[];
  badges?: GalleryBadge[];
};

const props = defineProps<Props>();

const activeIndex = ref(0);

const safeImages = computed(() =>
  (props.images ?? []).filter(
    (image): image is GalleryImage => Boolean(image?.src?.trim()),
  ),
);

const safeBadges = computed(() =>
  (props.badges ?? []).filter((badge) => Boolean(badge?.label?.trim())),
);

const currentImage = computed(() => safeImages.value[activeIndex.value] ?? null);

const hasMultipleImages = computed(() => safeImages.value.length > 1);

watch(
  safeImages,
  (nextImages) => {
    const maxIndex = Math.max(0, nextImages.length - 1);

    if (activeIndex.value > maxIndex) {
      activeIndex.value = maxIndex;
    }
  },
  { immediate: true },
);

function clampIndex(index: number): number {
  return Math.max(0, Math.min(index, safeImages.value.length - 1));
}

function goTo(index: number): void {
  activeIndex.value = clampIndex(index);
}

function goPrev(): void {
  if (!hasMultipleImages.value) return;
  goTo(activeIndex.value - 1);
}

function goNext(): void {
  if (!hasMultipleImages.value) return;
  goTo(activeIndex.value + 1);
}
</script>

<template>
  <section
    class="rounded-3xl border border-slate-200 bg-white p-4 shadow-sm sm:p-5"
    :aria-label="$t('properties:card.gallery.ariaLabel')"
  >
    <div
      class="relative overflow-hidden rounded-2xl border border-slate-200 bg-slate-50"
      tabindex="0"
      @keydown.left.prevent="goPrev"
      @keydown.right.prevent="goNext"
    >
      <div
        class="pointer-events-none absolute inset-0 bg-[radial-gradient(70%_60%_at_18%_14%,rgba(255,255,255,0.75),transparent_55%),radial-gradient(60%_55%_at_82%_4%,rgba(226,232,240,0.55),transparent_55%)]"
        aria-hidden="true"
      />

      <div class="relative aspect-[5/4] w-full">
        <img
          v-if="currentImage"
          :src="currentImage.src"
          :alt="currentImage.alt"
          class="h-full w-full object-cover"
        />

        <div
          v-if="safeBadges.length"
          class="absolute left-3 top-3 z-10 flex flex-wrap gap-2 sm:left-4 sm:top-4"
        >
          <span
            v-for="(badge, index) in safeBadges"
            :key="`${badge.label}-${index}`"
            class="inline-flex items-center rounded-full px-3 py-1 text-xs font-semibold ring-1 ring-inset backdrop-blur"
            :class="badge.class"
            :data-testid="badge.testId"
          >
            {{ badge.label }}
          </span>
        </div>

        <div
          class="absolute bottom-3 right-3 rounded-full border border-slate-200 bg-white/90 px-3 py-1 text-xs font-semibold text-slate-800 shadow-sm backdrop-blur"
          :aria-label="$t('properties:card.gallery.imageIndexAriaLabel')"
        >
          {{ safeImages.length ? `${activeIndex + 1} / ${safeImages.length}` : "—" }}
        </div>

        <template v-if="hasMultipleImages">
          <button
            type="button"
            class="absolute left-3 top-1/2 grid h-10 w-10 -translate-y-1/2 place-items-center rounded-full border border-slate-200 bg-white/90 text-lg text-slate-700 shadow-sm backdrop-blur transition hover:bg-white disabled:cursor-not-allowed disabled:opacity-40"
            :aria-label="$t('properties:card.gallery.previousImage')"
            :disabled="activeIndex === 0"
            @click="goPrev"
          >
            ‹
          </button>

          <button
            type="button"
            class="absolute right-3 top-1/2 grid h-10 w-10 -translate-y-1/2 place-items-center rounded-full border border-slate-200 bg-white/90 text-lg text-slate-700 shadow-sm backdrop-blur transition hover:bg-white disabled:cursor-not-allowed disabled:opacity-40"
            :aria-label="$t('properties:card.gallery.nextImage')"
            :disabled="activeIndex === safeImages.length - 1"
            @click="goNext"
          >
            ›
          </button>
        </template>

        <div
          aria-hidden="true"
          class="pointer-events-none absolute inset-0 bg-[linear-gradient(to_top,rgba(248,250,252,0.32),transparent_55%)]"
        />
      </div>
    </div>

    <div v-if="safeImages.length > 1" class="mt-3">
      <div class="flex snap-x snap-mandatory gap-2.5 overflow-x-auto pb-1 [-webkit-overflow-scrolling:touch]">
        <button
          v-for="(image, index) in safeImages"
          :key="`${image.src}-${index}`"
          type="button"
          class="shrink-0 snap-center overflow-hidden rounded-xl border bg-slate-50 transition focus:outline-none focus-visible:ring-2 focus-visible:ring-emerald-300"
          :class="
            index === activeIndex
              ? 'border-emerald-300 ring-2 ring-emerald-100'
              : 'border-slate-200 hover:border-slate-300'
          "
          :aria-label="`${$t('properties:card.gallery.thumbnailAriaLabel')} ${index + 1}`"
          @click="goTo(index)"
        >
          <div class="relative h-16 w-16 sm:h-20 sm:w-20">
            <img
              :src="image.src"
              :alt="image.alt"
              class="h-full w-full object-cover"
            />
          </div>
        </button>
      </div>
    </div>
  </section>
</template>
