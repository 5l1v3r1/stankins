import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { RecipesService } from '../recipes.service';
import { Recipe } from '../Recipe';
import { tap, switchMap, map, filter, distinctUntilChanged, debounceTime } from 'rxjs/operators';
import { Subject, Observable, fromEvent } from 'rxjs';
import { FormControl } from '@angular/forms';
@Component({
  selector: 'app-recipes-list',
  templateUrl: './recipes-list.component.html',
  styleUrls: ['./recipes-list.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})


export class RecipesListComponent implements OnInit {

  items = Array.from({length: 100000}).map((_, i) => `Item #${i}`);
  search: SearchRecipe;
  numberRecipes: number;
  findRecipes: Recipe[];
  searchRecipe = new FormControl();

  constructor(private recipeService: RecipesService) {

  }

  ngOnInit() {
    this.search = new SearchRecipe(this.recipeService);
    this.search.loadRecipes().subscribe(it => this.numberRecipes = it);

  this.searchRecipe.valueChanges.pipe(
    // filter(it => it != null && it.length > 1),
    debounceTime(100),
    distinctUntilChanged(),
    tap(it => {

      console.log(it);

    })
  ).subscribe(it => this.findRecipes = this.search.SearchRecipe(it)  );
  }

}

export class SearchRecipe {
  // private findRecipes=new  Subject<number>();

  private allRecipes: Recipe[];
  constructor(private recipeService: RecipesService) {

  }

  loadRecipes(): Observable<number> {

    return this.recipeService.GetStankinsAll()
    .pipe(
      tap(it => {
        return window.console.table(it);
      } ),
      map ((rec) => {
        const list = new Array<Recipe>();
        rec.forEach(element => {
          list.push(new Recipe(element));
        });
        this.allRecipes = list.sort((x, y) => x.name.localeCompare(y.name));
        return list.length;
      } )

    );
    // .subscribe(it => {
    //     this.allRecipes = it;
    //     this.findRecipes.next(it.length);
    //   });
  }
  // public findRecs(): Observable<number> {
  //   return this.findRecipes.asObservable();
  // }
  public SearchRecipe(s: string): Recipe[] {
    if (s == null || s.length === 0) {
      return null;
    }
    return this.allRecipes.filter(it => it.searchString().indexOf(s) > -1);
  }
}