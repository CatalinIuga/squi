<script setup lang="ts">
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import {
  ChevronLeftIcon,
  ChevronRightIcon,
  CodeIcon,
  DownloadIcon,
  ListFilter,
  PlusIcon,
  RefreshCw,
  SettingsIcon,
  SlidersHorizontal,
  XIcon,
} from "lucide-vue-next";
import { ref } from "vue";
import AgGridTable from "./components/AgGridTable.vue";

const currentTable = ref<string>("");
// const tables = ref<Array<string>>([]);
const openTables = ref<Array<string>>([]);

// Table reference for calling exposed methods
const tableRef = ref<InstanceType<typeof AgGridTable> | null>(null);

const discard = () => {
  tableRef.value?.discardChanges();
};
</script>

<template>
  <main class="mx-auto h-full flex-col overflow-hidden">
    <!-- Table selector -->
    <header
      class="mx-auto flex h-[50px] items-center gap-2 border-b-[1px] px-4"
    >
      <!-- Refresh button with icon -->
      <Button variant="outline" class="size-9 p-2" size="icon">
        <CodeIcon :size="16" />
      </Button>
      <!-- Buttons for all open tables -->
      <Button
        v-for="(table, id) in ['Customers']"
        :key="id"
        class="flex items-center gap-2 bg-slate-100 hover:bg-slate-100/80"
        variant="outline"
        :active="table === currentTable"
        @click="console.log('open table')"
      >
        <div class="">{{ table }}</div>
        <XIcon
          @click="
            (e) => {
              e.stopImmediatePropagation();
              console.log('stoped');
            }
          "
          class="hover:cursor- pr-0"
          :size="12"
        />
      </Button>
      <!-- Open table button -->
      <Button
        class="size-9 p-2"
        :class="
          openTables.length !== 0 ? 'bg-slate-100 hover:bg-slate-100/80' : ''
        "
        variant="outline"
        size="icon"
      >
        <PlusIcon :size="16" />
      </Button>

      <!-- Settings -->
      <div class="ml-auto flex items-center">
        <Button class="flex items-center gap-2" variant="outline" size="sm">
          <SettingsIcon :size="16" />
        </Button>
      </div>
    </header>

    <article class="h-full flex flex-col w-full">
      <!-- Table options -->
      <section class="flex h-14 items-center gap-2 px-4 py-3 border-b-[1px]">
        <!-- Filters -->
        <Button
          class="flex items-center text-sm gap-2"
          variant="outline"
          size="sm"
        >
          <ListFilter :size="16" />
          Filters
        </Button>
        <!-- Columns -->
        <Button
          class="flex items-center text-sm gap-2"
          variant="outline"
          size="sm"
        >
          <SlidersHorizontal :size="16" />
          Columns
        </Button>
        <!-- Add new -->
        <Button class="flex text-sm" variant="default" size="sm">
          Add record
        </Button>
        <!-- Save Changes -->
        <Button
          class="flex text-sm bg-green-600 hover:bg-green-600/80"
          variant="default"
          size="sm"
        >
          Save {{ 1 }} changes
        </Button>
        <!-- Discard Changes -->
        <Button
          @click="discard"
          v-if="tableRef?.discardedChanges.length! > 0"
          class="flex text-sm underline"
          variant="ghost"
          size="sm"
        >
          Discard
          {{
            tableRef?.discardedChanges.length +
            " " +
            (tableRef?.discardedChanges.length! > 1 ? "changes" : "change")
          }}
        </Button>
        <!-- Delete records -->
        <Button class="flex text-sm" variant="destructive" size="sm">
          Delete {{ 3 }} records
        </Button>

        <div class="ml-auto flex items-center gap-2">
          <!-- No. of rows -->
          <div class="">50 rows</div>
          <!-- Prev -->
          <Button
            class="flex items-center size-8 gap-1"
            variant="ghost"
            size="icon"
          >
            <ChevronLeftIcon :size="24" />
          </Button>

          <!-- Limit -->
          <TooltipProvider :delay-duration="300">
            <Tooltip>
              <TooltipTrigger>
                <Input class="w-20 h-8" default-value="50" />
              </TooltipTrigger>
              <TooltipContent class="bg-background border text-foreground">
                LIMIT
              </TooltipContent>
            </Tooltip>
          </TooltipProvider>

          <!-- Offset -->
          <TooltipProvider :delay-duration="300">
            <Tooltip>
              <TooltipTrigger>
                <Input class="w-20 h-8" default-value="0" />
              </TooltipTrigger>
              <TooltipContent class="bg-background border text-foreground">
                OFFSET
              </TooltipContent>
            </Tooltip>
          </TooltipProvider>

          <!-- Next -->
          <Button
            class="flex items-center gap-1 size-8"
            variant="ghost"
            size="icon"
          >
            <ChevronRightIcon :size="24" />
          </Button>
        </div>

        <!-- Refresh Button -->
        <Button
          class="flex items-center gap-1 size-8"
          variant="outline"
          size="icon"
        >
          <RefreshCw :size="16" />
        </Button>
        <!-- Download Button -->
        <Button
          class="flex items-center gap-1 size-8"
          variant="outline"
          size="icon"
        >
          <DownloadIcon :size="16" />
        </Button>
      </section>
      <!-- Table -->
      <output class="h-full overflow-auto pb-12">
        <AgGridTable ref="tableRef" />
      </output>
    </article>
  </main>
</template>
