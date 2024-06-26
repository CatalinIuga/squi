<script setup lang="ts">
import { Button } from "@/components/ui/button";
import { PlusIcon } from "lucide-vue-next";
</script>

<template>
  <Button class="size-8" variant="outline" size="icon">
    <PlusIcon :size="16" />
  </Button>
</template>
