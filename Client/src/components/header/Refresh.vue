<script setup lang="ts">
import { Button } from "@/components/ui/button";
import { RefreshCw } from "lucide-vue-next";
</script>

<template>
  <Button class="flex items-center gap-1 size-8" variant="outline" size="icon">
    <RefreshCw :size="16" />
  </Button>
</template>
