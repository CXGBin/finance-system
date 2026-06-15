import { create } from 'zustand';
import type { ModuleItem } from '@/types/auth.d';

interface ModuleState {
  modules: ModuleItem[];
  setModules: (modules: ModuleItem[]) => void;
  isModuleEnabled: (code: string) => boolean;
  toggleModule: (code: string, enabled: boolean) => void;
}

export const useModuleStore = create<ModuleState>((set, get) => ({
  modules: [],
  setModules: (modules) => set({ modules }),
  isModuleEnabled: (code) => {
    const mod = get().modules.find((m) => m.code === code);
    return mod ? mod.enabled : false;
  },
  toggleModule: (code, enabled) =>
    set((state) => ({
      modules: state.modules.map((m) => (m.code === code ? { ...m, enabled } : m)),
    })),
}));
