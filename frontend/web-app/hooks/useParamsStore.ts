import { create } from "zustand";

//put possible parameters for crud queries
type State = {
  //current page
  pageNumber: number;
  //how many items to display
  pageSize: number;

  pageCount: number;

  //sent to api
  searchTerm: string;
  //displayed value in search bar
  searchValue: string;
  orderBy: string;
  filterBy: string;
};

type Actions = {
  //Partial means any number of parameters all are optional you can update one only, or update more than one
  setParams: (params: Partial<State>) => void;
  //to reset the param
  reset: () => void;
  setSearchValue: (value: string) => void;
};

const initialState: State = {
  pageNumber: 1,
  pageSize: 12,
  pageCount: 1,
  searchTerm: "",
  searchValue: "",
  orderBy: "make",
  filterBy: "live",
};
//create is zustand method
//set is what to use to manipulate the state
export const useParamsStore = create<State & Actions>()((set) => ({
  //spread the initialstate so we can manipulate it
  ...initialState,

  //partial means it can be any parameter, parameters are optional
  setParams: (newParams: Partial<State>) => {
    //state is the existing state stored
    set((state) => {
      //
      if (newParams.pageNumber) {
        //maintain the rest of state and change only the pagenumber
        return { ...state, pageNumber: newParams.pageNumber };
      }
      // If pageNumber is NOT provided, update the state with newParams
      // AND reset pageNumber to 1
      else {
        return { ...state, ...newParams, pageNumber: 1 };
      }
    });
  },
  //reset the values to initialstate
  reset: () => set(initialState),

  //set the value pf displayed search
  setSearchValue: (value: string) => {
    set({ searchValue: value });
  },
}));
